//
//  Copyright 2019, Desert Software Solutions Inc.
//    ValueBagExtensions.cs: 
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

namespace DesertSoftware.Solutions.Dynamix.Extensions
{
    /// <summary>
    /// Provides extension methods offering ValueBag functionality conveniences
    /// </summary>
    static public class ValueBagExtensions
    {
        /// <summary>
        /// Returns the <see cref="ValueBag"/> of the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        static public ValueBag ToValueBag(this object instance) {
            if (instance == null)
                return null;

            ValueBag bag = instance as ValueBag;

            if (bag != null)
                return bag;

            return new ValueBag(instance);
        }
    }
}
