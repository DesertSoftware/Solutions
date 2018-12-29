//
//  Copyright 2013, Desert Software Solutions Inc.
//    CharExtensions.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Extensions/CharExtensions.cs
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
    static public class CharExtensions
    {
        /// <summary>
        /// Returns true if any one of the values equals the char
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <param name="chars">The char values.</param>
        /// <returns></returns>
        static public bool In(this char ch, params char[] chars) {
            foreach (var value in chars)
                if (ch == value)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns true if any one of the char values of the specified string equals the char
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <param name="chars">The chars.</param>
        /// <returns></returns>
        static public bool In(this char ch, string chars) {
            return CharExtensions.In(ch, (chars ?? "").ToCharArray());
        }

        /// <summary>
        /// Returns the result of the specified comparison evaluation.
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <param name="comparison">The comparison evaluator.</param>
        /// <returns></returns>
        static public bool In(this char ch, Func<char, bool> comparison) {
            if (comparison == null)
                return false;

            return comparison(ch);
        }

        /// <summary>
        /// Returns true if none of the values equals the char
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <param name="chars">The char values.</param>
        /// <returns></returns>
        static public bool NotIn(this char ch, params char[] chars) {
            foreach (var value in chars)
                if (ch == value)
                    return false;

            return true;
        }

        /// <summary>
        /// Returns true if none of the char values in the specified string equals the char
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <param name="chars">The char string values.</param>
        /// <returns></returns>
        static public bool NotIn(this char ch, string chars) {
            return CharExtensions.NotIn(ch, (chars ?? "").ToCharArray());
        }

        /// <summary>
        /// Returns the result of the specified comparison evaluation.
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <param name="comparison">The comparison evaluator.</param>
        /// <returns></returns>
        static public bool NotIn(this char ch, Func<char, bool> comparison) {
            if (comparison == null)
                return false;

            return comparison(ch);
        }
    }
}
