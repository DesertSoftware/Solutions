//
//  Copyright 2015, Desert Software Solutions Inc.
//    MigrationRunner.cs:
//      https://github.com/DesertSoftware/Solutions
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
using System.IO;
using System.Linq;
using System.Text;

using FluentMigrator.Exceptions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Extensions;
using FluentMigrator.Runner.Initialization;

namespace DesertSoftware.Solutions.Data.Migrations.Runners
{
    public class MigrationRunner
    {
        /// <summary>
        /// Runs the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="database">The database.</param>
        /// <param name="writer">The writer.</param>
        static public void Run(string target, string connection, string database = "SqlServer", TextWriter writer = null) {
            Run(new RunnerContext { Targets = new string[] { target }, Connection = connection, Database = database }, writer);
        }

        /// <summary>
        /// Rollbacks the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="version">The version.</param>
        /// <param name="task">The task.</param>
        /// <param name="database">The database.</param>
        /// <param name="writer">The writer.</param>
        static public void Rollback(string target, string connection, long version, string task = "migrate:down", string database = "SqlServer", TextWriter writer = null) {
            Run(new RunnerContext { Targets = new string[] { target }, Connection = connection, Database = database, Task = task, Version = version }, writer);
        }

        /// <summary>
        /// Runs the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="writer">The writer.</param>
        static public void Run(RunnerContext context, TextWriter writer = null) {
            if (writer == null)
                writer = new StringWriter();

            IAnnouncer announcer = new DataWriter(writer);
            var runnerContext = new FluentMigrator.Runner.Initialization.RunnerContext(announcer)
            {
                ApplicationContext = context.ApplicationContext,
                Database = context.Database,
                Connection = context.Connection,
                Targets = context.Targets,
                PreviewOnly = context.PreviewOnly,
                Namespace = context.Namespace,
                NestedNamespaces = context.NestedNamespaces,
                Task = context.Task,
                Version = context.Version,
                Steps = context.Steps,
                WorkingDirectory = context.WorkingDirectory,
                Profile = context.Profile,
                Tags = context.Tags,
                Timeout = context.Timeout,
                TransactionPerSession = context.TransactionPerSession,
            };

            try {
                new TaskExecutor(runnerContext).Execute();
            } catch (ProcessorFactoryNotFoundException ex) {
                announcer.Error("While executing migrations the following error was encountered: {0}", ex.Message);
                throw;
            } catch (Exception e) {
                announcer.Error("While executing migrations the following error was encountered: {0}, {1}", e.Message, e.StackTrace);
                throw;
            } finally {
                //if (outputWriter != null)
                //    outputWriter.Dispose();
            }
        }
    }

    public class RunnerContext
    {
        public string Database { get; set; }
        public string Connection { get; set; }
        public string[] Targets { get; set; }
        public bool PreviewOnly { get; set; }
        public string Namespace { get; set; }
        public bool NestedNamespaces { get; set; }
        public string Task { get; set; }
        public long Version { get; set; }
        public long StartVersion { get; set; }
        public bool NoConnection { get; set; }
        public int Steps { get; set; }
        public string WorkingDirectory { get; set; }
        public string Profile { get; set; }
        public int Timeout { get; set; }
        public string ConnectionStringConfigPath { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public bool TransactionPerSession { get; set; }
        public string ProviderSwitches { get; set; }
        public object ApplicationContext { get; set; }
    }

    public class DataWriter : FluentMigrator.Runner.Announcers.TextWriterAnnouncer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public DataWriter(TextWriter writer) : base(writer) { }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="escaped">if set to <c>true</c> [escaped].</param>
        public override void Write(string message, bool escaped) {
            base.Write(message, false);
        }
    }
}
