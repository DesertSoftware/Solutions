//
//  Copyright 2015, Desert Software Solutions Inc.
//    CronSchedule.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Service/Cron/CronSchedule.cs
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NCrontab;

namespace DesertSoftware.Solutions.Service.Cron
{
    public class CronSchedule
    {
        // hourly
        // every hour
        // every minute
        // every 1 hour
        // every 3 minutes
        // every day
        // every day @ noon
        // every day @ midmight
        // every day @ 1am
        // daily 
        // daily @ noon
        // daily @ midnight
        // daily @ 1am
        // every week on monday @ midnight
        // weekly on monday @ midnight
        // every month @ 1am
        // every month on monday
        // every month on monday @ midnight

        //static private string ParseDaily()

        public class Time
        {
            public int Hour { get; set; }
            public int Minute { get; set; }
        }

        static private Time ParseTime(string schedule, string time) {
            string hour = time;
            int atHour = 0;
            int atMinute = 0;
            int ampm = hour.EndsWith("pm") || schedule.ToLower().Trim().EndsWith("pm") ? 12 : 0;   // handle the case of 1pm and 1 pm

            if (string.IsNullOrWhiteSpace(time))
                return new Time { Hour = 0, Minute = 0 };

            if (hour.EndsWith("am") || hour.EndsWith("pm"))
                hour = hour.Substring(0, hour.Length - 2);

            if (hour.StartsWith("midnight"))
                hour = "0";

            if (hour.StartsWith("noon"))
                hour = "12";

            string[] hourAndMinute = hour.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(hourAndMinute[0], out atHour);

            atHour += ampm;
            if (hourAndMinute.Length > 1)
                int.TryParse(hourAndMinute[1], out atMinute);

            return new Time
            {
                Hour = atHour < 24 ? atHour : 12,   // 12 pm (noon) adds up to 24 in this algorithm (adjust to 12)
                Minute = atMinute
            };
        }

        // handles parsing every dayofweek schedules
        static public string ParseEveryDayOfWeek(string schedule, string[] words, string dayOfWeek) {
            string word = "";

            if (words.Length < 3)
                return string.Format("0 0 * * {0}", dayOfWeek);

            word = (words[2].Trim() == "at" || words[2].Trim() == "@") && words.Length > 3
                ? words[3].Trim()
                : words[2].Trim();

            if (word.StartsWith("@"))
                word = word.Substring(1);

            Time time = ParseTime(schedule, word);

            return string.Format("{0} {1} * * {2}", time.Minute, time.Hour, dayOfWeek);
        }

        // every day
        // every day @ 1:15 am
        // every day at 1:15am
        static public string ParseEveryDay(string schedule, string[] words) {
            string word = "";

            if (words.Length < 3)
                return "0 0 * * *";

            word = (words[2].Trim() == "at" || words[2].Trim() == "@") && words.Length > 3
                ? words[3].Trim()
                : words[2].Trim();

            if (word.StartsWith("@"))
                word = word.Substring(1);

            Time time = ParseTime(schedule, word);

            return string.Format("{0} {1} * * *", time.Minute, time.Hour);
        }

        /// <summary>
        /// Parses the specified schedule into a well formed crontab expression.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <returns></returns>
        static public string Parse(string schedule) {
            const string HOURLY = "0 * * * *";
            const string EVERY_MINUTE = "* * * * *";
            const string DAILY = "0 0 * * *";
            const string WEEKLY = "0 0 * * sun";
            const string MONTHLY = "0 0 1 * *";

            int increment = 1;
            string[] words = schedule.ToLower().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string word = words[0].Trim();

            // hourly
            if (word.StartsWith("hour"))
                return HOURLY;

            // daily
            // daily @ 1am
            // daily at 1am
            // daily at 12:45am
            if (word.StartsWith("daily")) {
                if (words.Length < 2)
                    return DAILY;

                string[] dailyWords = new string[words.Length + 1];

                dailyWords[0] = "every";
                dailyWords[1] = "day";
                Array.Copy(words, 1, dailyWords, 2, words.Length - 1);

                return ParseEveryDay(schedule, dailyWords);
            }

            if (word.StartsWith("every")) {
                word = words.Length > 1 ? words[1].Trim() : "";

                switch (word) {
                    case "minute": return EVERY_MINUTE;
                    case "hour": return HOURLY;
                    case "day": return ParseEveryDay(schedule, words);
                    case "week": return WEEKLY;     // TODO: implement every week options
                    case "month": return MONTHLY;   // TODO: implement wevery month options

                    // every mon @ 1pm
                    // every monday @ 1am
                    // every monday at 3am
                    case "mon":
                    case "tue":
                    case "wed":
                    case "thu":
                    case "fri":
                    case "sat":
                    case "sun":
                    case "monday":
                    case "tuesday":
                    case "wednesday":
                    case "thursday":
                    case "friday":
                    case "saturday":
                    case "sunday":
                        return ParseEveryDayOfWeek(schedule, words, word.Substring(0, 3));

                    // every xx minute(s), every xx hour(s)
                    default:
                        word = words.Length > 2 ? words[2].Trim() : "";
                        int.TryParse(words[1], out increment);
                        break;
                }

                if (word.StartsWith("hour"))
                    return string.Format("0 */{0} * * *", increment);

                if (word.StartsWith("minute"))
                    return string.Format("*/{0} * * * *", increment);
            }

            if (word.StartsWith("month")) {
                if (words.Length < 2)
                    return MONTHLY;

                // monthly on dayofweek @ time
                // monthly @ time
                // TODO: implement monthly options
            }

            if (word.StartsWith("week")) {
                if (words.Length < 2)
                    return WEEKLY;

                // weekly on dayofweek
                // weekly on dayofweek @ time
                // TODO: immplement weekly options
            }

            // nothing special recoognized in the schedule, return it
            return schedule;
        }

        /// <summary>
        /// Gets the next scheduled occurrence from the specified start time.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="start">The start date time.</param>
        /// <returns></returns>
        static public DateTime GetNextOccurrence(string schedule, DateTime start) {
            return CrontabSchedule.Parse(CronSchedule.Parse(schedule)).GetNextOccurrence(start);
        }

        /// <summary>
        /// Gets the next scheduled occurrence within the specified start and end time period.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="start">The start date time.</param>
        /// <param name="end">The end date time.</param>
        /// <returns></returns>
        static public DateTime GetNextOccurrence(string schedule, DateTime start, DateTime end) {
            return CrontabSchedule.Parse(CronSchedule.Parse(schedule)).GetNextOccurrence(start, end);
        }

        /// <summary>
        /// Gets the scheduled occurrences within the specified start and end time period.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="start">The start date time.</param>
        /// <param name="end">The end date time.</param>
        /// <returns></returns>
        static public DateTime[] GetOccurrences(string schedule, DateTime start, DateTime end) {
            return CrontabSchedule.Parse(CronSchedule.Parse(schedule)).GetNextOccurrences(start, end).ToArray();
        }
    }
}