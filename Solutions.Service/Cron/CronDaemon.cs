//
//  Copyright 2015, Desert Software Solutions Inc.
//    CronDaemon.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Security/MD5Checksum.cs
//      adapted from https://github.com/sergeyt/CronDaemon
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NCrontab;

namespace DesertSoftware.Solutions.Service.Cron
{
    /// <summary>
    /// Cron scheduling daemon.
    /// </summary>
    public sealed class CronDaemon<T> : IDisposable
    {
        private readonly Action<T> execute;
        private readonly Func<T, T> fork;
        private readonly List<CancellationTokenSource> cancellations = new List<CancellationTokenSource>();

        /// <summary>
        /// Initializes new instance of <see cref="CronDaemon{T}"/>.
        /// </summary>
        /// <param name="execute">The job handler.</param>
        /// <param name="fork">The function to fork job instance on every recurrence.</param>
        public CronDaemon(Action<T> execute, Func<T, T> fork = null) {
            if (execute == null) throw new ArgumentNullException("execute");

            this.execute = execute;
            this.fork = fork ?? Fork;
        }

        private static T Fork(T item) {
            var cloneable = item as ICloneable;

            return cloneable != null
                ? (T)cloneable.Clone()
                : item;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            foreach (var cancellation in this.cancellations)
                cancellation.Cancel(false);

            this.cancellations.Clear();
        }

        public void Dispose() {
            Stop();
        }

        /// <summary>
        /// Adds specified job to <see cref="CronDaemon{T}" /> queue with given cron expression and maximum number of repetitions.
        /// </summary>
        /// <param name="job">The job definition.</param>
        /// <param name="cronExpression">Specifies cron expression.</param>
        /// <param name="repeatCount">Specifies maximum number of job recurrence.</param>
        /// <param name="runNow">if set to <c>true</c> run now.</param>
        /// <param name="useUTC">if set to <c>true</c> use Universal Time Coordinates.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">repeatCount</exception>
        public void Add(T job, string cronExpression, int repeatCount, bool runNow = false, bool useUTC = false) {
            if (repeatCount < 0) throw new ArgumentOutOfRangeException("repeatCount");

            // 0 means once like in java quartz
            if (repeatCount == 0) repeatCount = 1;

            var cancellation = new CancellationTokenSource();
            var crontab = CronSchedule.ToCrontabSchedule(cronExpression);

            Func<DateTime, DateTime?> schedule = time => {
                if (cancellation.IsCancellationRequested)
                    return null;

                if (--repeatCount >= 0)
                    return crontab.GetNextOccurrence(time);

                return null;
            };

            Action run = async () => {
                if (runNow)
                    this.execute(this.fork(job));

                while (true) {
                    var now = useUTC ? DateTime.UtcNow : DateTime.Now;
                    var nextOccurrence = schedule(now);

                    // nextOccurrence of null means the schedule has ended. stop job
                    if (nextOccurrence == null)
                        break;

                    // wait prescribed period of time until time to execute
                    try {
                        await Task.Delay(nextOccurrence.Value - now, cancellation.Token);
                    } catch (TaskCanceledException) {
                        break;
                    } catch (Exception) {
                        throw;
                    }

                    if (cancellation.IsCancellationRequested)
                        break;

                    // time to execute the job
                    this.execute(this.fork(job));
                }
            };

            // start up the task
            Task.Run(run, cancellation.Token);
            this.cancellations.Add(cancellation);
        }
    }

    public static class CronDaemon
    {
        /// <summary>
        /// Starts the specified daemon.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="execute">The execute.</param>
        /// <param name="fork">The fork.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">execute</exception>
        public static CronDaemon<T> Start<T>(Action<T> execute, Func<T, T> fork = null) {
            if (execute == null) throw new ArgumentNullException("execute");

            return new CronDaemon<T>(execute, fork);
        }
    }
}