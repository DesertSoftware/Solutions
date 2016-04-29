//
//  Copyright 2015, Desert Software Solutions Inc.
//    DynamicXmlFormatter.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Formatters/DynamicXmlFormatter.cs
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
using System.Xml;
using System.Xml.Linq;

namespace DesertSoftware.Solutions.Formatters
{
    /// <summary>
    /// Dynamic object Xml formatting methods.
    /// </summary>
    public static class DynamicXmlFormatter
    {
        /// <summary>
        /// Defines the simple types that is directly writeable to XML.
        /// </summary>
        private static readonly Type[] simpleTypes = new[] { typeof(string), typeof(DateTime), typeof(Enum), typeof(decimal), typeof(Guid) };

        /// <summary>
        /// Determines whether [is simple type] [the specified type].
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<c>true</c> if [is simple type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSimpleType(this Type type) {
            return type.IsPrimitive || simpleTypes.Contains(type);
        }

        /// <summary>
        /// Converts the specified dynamic object to XML.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object.</param>
        /// <param name="element">The element name.</param>
        /// <returns>
        /// Returns an Xml representation of the dynamic object.
        /// </returns>
        public static XElement SerializeToXml(dynamic dynamicObject, string element = "object") {
            var members = dynamicObject as IDictionary<string, object>; ;
            var root = new XElement(XmlConvert.EncodeName(element ?? "object"));

            if (members != null)
                root.Add(from prop in members
                         let name = XmlConvert.EncodeName(prop.Key)
                         let xml = prop.Value.ToXml(name)
                         where xml != null
                         select xml);

            return root;
        }

        /// <summary>
        /// Generates an XML string from the dynamic object.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object.</param>
        /// <returns>Returns an XML string.</returns>
        public static string ToXmlString(dynamic dynamicObject, string root = "object") {
            return DynamicXmlFormatter.SerializeToXml(dynamicObject, root).ToString();
        }

        /// <summary>
        /// Converts an enumerable list to XML.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        static private List<XElement> ListToXml(IEnumerable<dynamic> list, string element = "object") {
            var elements = new List<XElement>();

            if (list != null)
                foreach (var xml in from e in list
                                    select ToXml(e, element ?? "object"))
                    elements.AddRange(xml);

            return elements;
        }

        /// <summary>
        /// Converts a simple value type to XML.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        static private List<XElement> SimpleTypeToXml(object value, string element = "object") {
            var elements = new List<XElement>();

            elements.Add(new XElement(element ?? "object", value));

            return elements;
        }

        /// <summary>
        /// Converts a key/value dictionary to XML.
        /// </summary>
        /// <param name="members">The members.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        static private List<XElement> DictionaryToXml(this IDictionary<string, object> members, string element = "object") {
            var elements = new List<XElement>();
            var xelement = new XElement(XmlConvert.EncodeName(element ?? "object"));

            if (members != null) {
                xelement.Add(from prop in members
                             let name = XmlConvert.EncodeName(prop.Key)
                             let xml = prop.Value.ToXml(name)
                             where xml != null
                             select xml);

                elements.Add(xelement);
            }

            return elements;
        }

        /// <summary>
        /// Conbverts an object to XML.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        static private List<XElement> ObjectToXml(object value, string element = "object") {
            var elements = new List<XElement>();
            var xelement = new XElement(XmlConvert.EncodeName(element ?? "object"));

            xelement.Add(from prop in value.GetType().GetProperties()
                         let name = XmlConvert.EncodeName(prop.Name)
                         let xml = prop.GetValue(value, null).ToXml(name)
                         where xml != null
                         select xml);

            elements.Add(xelement);

            return elements;

        }

        /// <summary>
        /// Converts an anonymous type to an XElement.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="element">The element name.</param>
        /// <returns>Returns the object as it's XML representation in an XElement.</returns>
        private static List<XElement> ToXml(this object input, string element = "object") {
            if (input == null)
                return null;

            // simple type
            if (input.GetType().IsSimpleType())
                return SimpleTypeToXml(input, element);

            // array
            // we can check for array and process it accordingly but 
            //   theoretically arrays should be enumerable and processed in next case
            // if (input.GetType().IsArray) ... return ArrayToXml((Array)input, element ?? "object");

            // list/enumerable
            IEnumerable<dynamic> list = input as IEnumerable<dynamic>;
            if (list != null)
                return ListToXml(list, element);


            // expando/dynamic
            IDictionary<string, object> members = input as IDictionary<string, object>;
            if (members != null)
                return DictionaryToXml(members, element ?? "object");

            // complex object
            return ObjectToXml(input, element ?? "object");
        }
    }
}
