//
//  Copyright 2013, Desert Software Solutions Inc.
//    BoolExtensions.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Extensions/BoolExtensions.cs
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
    /// <summary>
    /// The BoolExtensions class contains a set of extension methods that provide fluent readability 
    /// </summary>
    static public class BoolExtensions
    {
        /// <summary>
        /// Returns true if the boolean value is true.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        static public bool IsOn(this bool value) {
            return value;
        }

        /// <summary>
        /// Returns true if the boolean value is false.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        static public bool IsOff(this bool value) {
            return !value;
        }

        /// <summary>
        /// Returns true if the boolean value is true.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        static public bool IsEnabled(this bool value) {
            return value;
        }

        /// <summary>
        /// Returns true if the boolean value is false.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        static public bool IsDisabled(this bool value) {
            return !value;
        }
    }
}
