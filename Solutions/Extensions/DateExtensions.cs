//
//  Copyright 2014, Desert Software Solutions Inc.
//    DateExtensions.cs: 
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

namespace DesertSoftware.Solutions.Extensions
{
    static public class DateExtensions
    {
        /// <summary>
        /// create an array of seqeuntial dates from a given startDate with a sequence length of a given number of days.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="days">The days.</param>
        /// <returns></returns>
        static public DateTime[] Sequence(this DateTime startDate, int days) {
            var dates = new List<DateTime>();

            for (var day = 0; day < days; day++)
                dates.Add(startDate.AddDays(day).Date);

            return dates.ToArray();
        }
    }
}
