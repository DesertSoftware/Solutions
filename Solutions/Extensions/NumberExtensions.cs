//
//  Copyright 2013, Desert Software Solutions Inc.
//    DateExtensions.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Extensions/NumberExtensions.cs
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
    static public class NumberExtensions
    {
        /// <summary>
        /// Returns a sequence of numbers up to the specified maximum.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        static public int[] Sequence(this int min, int max, int step = 1) {
            var values = new List<int>();

            for (var value = min; value <= max; value += step)
                values.Add(value);

            return values.ToArray();
        }

        /// <summary>
        /// Returns a defualt value when the specified single value is indeterminate (NaN, Inifinity, etc).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        static public Single DefaultWhenIndeterminate(this Single value, Single defaultValue = 0) {
            if ((System.Single.IsNaN(value)) || (System.Single.IsInfinity(value)) || (value <= System.Single.Epsilon))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// Returns a defualt value when the specified single value is indeterminate (NaN, Inifinity, etc).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        static public double DefaultWhenIndeterminate(this double value, double defaultValue = 0) {
            if ((System.Double.IsNaN(value)) || (System.Double.IsInfinity(value)) || (value <= System.Double.Epsilon))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// Returns <c>true</c> if the value is between the specified start and end values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="inclusive">if set to <c>true</c> [inclusive].</param>
        /// <returns></returns>
        static public bool Between(this double value, int start, int end, bool inclusive = true) {
            if (inclusive)
                return value >= start && value <= end;

            return value > start && value < end;
        }

        /// <summary>
        /// Returns <c>true</c> if the value is between the specified start and end values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="inclusive">if set to <c>true</c> [inclusive].</param>
        /// <returns></returns>
        static public bool Between(this int value, int start, int end, bool inclusive = true) {
            if (inclusive)
                return value >= start && value <= end;

            return value > start && value < end;
        }

        /// <summary>
        /// Returns <c>true</c> if the value is found to exist in the specified set of values
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        static public bool In(this int value, params int[] values) {
            foreach (var v in values)
                if (value == v)
                    return true;

            return false;
        }
    }
}
