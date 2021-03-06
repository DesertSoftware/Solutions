﻿//
//  Copyright 2018, Desert Software Solutions Inc.
//    Loggers.cs: 
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
using System.Linq;
using System.Text;

namespace DesertSoftware.Solutions.Anonymous
{
    /// <summary>
    /// The contents of this class provide Action compatible implementations of
    /// loggers that can be used in places where a logger of the form Action<int, string>
    /// is requested.
    /// </summary>
    public class Loggers
    {
        /// <summary>
        /// Fulfills the function of an action pattern logger without performing
        /// any actual logging.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="message">The message.</param>
        static public void NullLogger(int severity, string message) { }

        /// <summary>
        /// The StringLogger class provides functionality to log event information
        /// to a string.
        /// </summary>
        public class StringLogger
        {
            protected const int DEBUG = 0;
            protected const int INFO = 1;
            protected const int WARN = 2;
            protected const int ERROR = 3;
            protected const int FATAL = 4;

            protected StringBuilder s = new StringBuilder();

            /// <summary>
            /// Returns the textual representation of the specified severity suitable for logging/display.
            /// Override this method to modify the severity text written to the logger.
            /// </summary>
            /// <param name="severity">The severity.</param>
            /// <returns></returns>
            protected virtual string SeverityToText(int severity) {
                switch (severity) {
                    case DEBUG: return "DEBUG ";
                    case INFO: return "INFO  ";
                    case WARN: return "WARN  ";
                    case ERROR: return "ERROR ";
                    case FATAL: return "FATAL ";
                    default: return "      ";
                }
            }

            /// <summary>
            /// Logs the specified message with the specified severity.
            /// </summary>
            /// <param name="severity">The severity.</param>
            /// <param name="message">The message.</param>
            public virtual void Log(int severity, string message) {
                this.s.AppendFormat("{0} {1}\n", SeverityToText(severity), message);
            }

            /// <summary>
            /// Logs the specified message with a DEBUG severity.
            /// </summary>
            /// <param name="message">The message.</param>
            public void Debug(string message) {
                Log(DEBUG, message);
            }

            /// <summary>
            /// Logs the specified message with an INFO severity.
            /// </summary>
            /// <param name="message">The message.</param>
            public void Info(string message) {
                Log(INFO, message);
            }

            /// <summary>
            /// Logs the specified message with a WARN severity.
            /// </summary>
            /// <param name="message">The message.</param>
            public void Warn(string message) {
                Log(WARN, message);
            }

            /// <summary>
            /// Logs the specified message with an ERROR severity.
            /// </summary>
            /// <param name="message">The message.</param>
            public void Error(string message) {
                Log(ERROR, message);
            }

            /// <summary>
            /// Logs the specified message with a FATAL severity.
            /// </summary>
            /// <param name="message">The message.</param>
            public void Fatal(string message) {
                Log(FATAL, message);
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents the log entries of this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString() {
                return this.s.ToString();
            }
        }

        /// <summary>
        /// The HtmlListLogger provides functionality to log event entry information
        /// to a string encoded as html li elements.
        /// </summary>
        public class HtmlListLogger : StringLogger
        {
            /// <summary>
            /// Returns the CSS class of the specified severity.
            /// </summary>
            /// <param name="severity">The severity.</param>
            /// <returns></returns>
            protected string SeverityClass(int severity) {
                switch (severity) {
                    case DEBUG: return "debug";
                    case INFO: return "info";
                    case WARN: return "warn";
                    case ERROR: return "error";
                    case FATAL: return "fatal";
                    default: return severity.ToString();
                }
            }

            /// <summary>
            /// Logs the specified message with the specified severity.
            /// </summary>
            /// <param name="severity">The severity.</param>
            /// <param name="message">The message.</param>
            public override void Log(int severity, string message) {
                base.s.AppendFormat("<li class='log-{0}'>{1} {2}</li>", SeverityClass(severity).Trim().ToLower(), SeverityToText(severity), message);
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents the log entries of this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString() {
                return base.ToString();
            }
        }
    }
}
