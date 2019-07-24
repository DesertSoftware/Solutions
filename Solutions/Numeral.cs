//
//  Copyright 2014, Desert Software Solutions Inc.
//    Numeral.cs:
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

namespace DesertSoftware.Solutions
{
    public class Numeral
    {
        /// <summary>
        /// Returns the least value of the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        static public T Min<T>(params T[] values) {
            T minValue = values[0];

            foreach (T value in values)
                if ((dynamic)value < (dynamic)minValue)
                    minValue = value;

            return minValue;
        }

        /// <summary>
        /// Returns the greatest value of the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        static public T Max<T>(params T[] values) {
            T maxValue = values[0];

            foreach (T value in values)
                if ((dynamic)value > (dynamic)maxValue)
                    maxValue = value;

            return maxValue;
        }
    }
}
