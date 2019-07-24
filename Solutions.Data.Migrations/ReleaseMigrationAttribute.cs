//
//  Copyright 2015, Desert Software Solutions Inc.
//    ReleaseMigrationAttribute.cs:
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

using FluentMigrator;

namespace DesertSoftware.Solutions.Data.Migrations
{
    /// <summary>
    /// Mark all migrations with this INSTEAD of [Migration].
    /// </summary>
    public class ReleaseMigrationAttribute : MigrationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReleaseMigrationAttribute"/> class.
        /// </summary>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="build">The build.</param>
        /// <param name="description">The description.</param>
        public ReleaseMigrationAttribute(int major, int minor, int year, int month, int day, int build = 0, string description = "")
            : base(ReleaseVersion(major, minor, year, month, day, build), ReleaseDescription(major, minor, year, month, day, build, description)) {

        }

        static private string ReleaseDescription(int major, int minor, int year, int month, int day, int build, string description) {
            return string.Format(
                "{0}.{1}.{2}.{3}{4:00}{5}: data migration{6}",
                major, minor, year, month, day,
                (build > 0 ? build.ToString(" (0)") : ""),
                (!string.IsNullOrWhiteSpace(description) ? string.Format(" - {0}", description) : ""));
        }

        static private long ReleaseVersion(int major, int minor, int year, int month, int day, int build) {
            string revision = string.Format("{0}{1}{2}", month, day, (build > 0 ? build.ToString() : ""));

            if (int.Parse(revision) > 65535)
                throw new ArgumentOutOfRangeException(
                    string.Format("month({0}), day({1}), build({2}) exceeds 65535 '{3}'", month, day, build, revision));

            return long.Parse(string.Format("{0}{1}{2}{3:00}{4:00}{5}", major, minor, year, month, day, (build > 0 ? build.ToString() : "")));
        }
    }
}

