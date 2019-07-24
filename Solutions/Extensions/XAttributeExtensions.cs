//
//  Copyright 2017, Desert Software Solutions Inc.
//    XAttributeExtensions.cs:
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
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;

namespace DesertSoftware.Solutions.Extensions
{
    static public class XAttributeExtensions
    {
        /// <summary>
        /// Returns the first named attribute found in the specified element.
        /// Search is performed in named order.
        /// </summary>
        /// <param name="element">The element to search in.</param>
        /// <param name="attribute">The attribute(s) to find.</param>
        /// <returns></returns>
        static public XAttribute Attribute(this XElement element, params string[] attribute) {
            XAttribute result = null;

            foreach (var attr in attribute) {
                result = element.Attribute(attr);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
