//
//  Copyright 2014, Desert Software Solutions Inc.
//    XElementExtensions.cs:
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Extensions/XElementExtensions.cs
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace DesertSoftware.Solutions.Extensions
{
    static public class XElementExtensions
    {
        static private dynamic TypedValue(string value) {
            // attempt to determine the basic value type
            int intValue = 0;
            double doubleValue = 0;
            decimal decimalValue = 0;
            float floatValue = 0;

            if (int.TryParse(value, out intValue))
                return intValue;

            if (double.TryParse(value, out doubleValue))
                return doubleValue;

            if (decimal.TryParse(value, out decimalValue))
                return decimalValue;

            if (float.TryParse(value, out floatValue))
                return floatValue;

            return value;
        }

        static public dynamic ToDynamic(this XElement element) {
            var item = new ExpandoObject();
            IDictionary<string, object> properties = item;

            // Add all of the sub elements if we have any 
            if (element.HasElements) {
                var uniqueElements = element.Elements().Where(el => element.Elements().Count(el2 => el2.Name.LocalName.Equals(el.Name.LocalName)) == 1);
                var repeatedElements = element.Elements().Except(uniqueElements);

                foreach (var repeatedElementGroup in repeatedElements.GroupBy(re => re.Name.LocalName).OrderBy(el => el.Key)) {
                    var list = new List<dynamic>();

                    foreach (var repeatedElement in repeatedElementGroup)
                        list.Add(ToDynamic(repeatedElement));

                    properties.Add(XmlConvert.DecodeName(repeatedElementGroup.Key), list);
                }

                foreach (var uniqueElement in uniqueElements.OrderBy(el => el.Name.LocalName))
                    properties.Add(XmlConvert.DecodeName(uniqueElement.Name.LocalName), ToDynamic(uniqueElement));
            }

            // Add attributes, if any
            if (element.Attributes().Any())
                foreach (var attribute in element.Attributes())
                    if (!attribute.IsNamespaceDeclaration)
                        properties.Add(XmlConvert.DecodeName(attribute.Name.LocalName), TypedValue(attribute.Value));

            if (!element.HasElements && !string.IsNullOrWhiteSpace(element.Value))
                properties.Add(XmlConvert.DecodeName(element.Name.LocalName), element.Value);

            if (element.HasElements || element.HasAttributes)
                return item;

            return TypedValue(element.Value);
        }
    }
}
