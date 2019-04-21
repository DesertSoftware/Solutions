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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;

namespace DesertSoftware.Solutions.Extensions
{
    public class DynamicElement : DynamicObject, IDictionary<string, object>
    {
        private XElement element;
        private bool allPropertiesLoaded = false;
        private Dictionary<string, dynamic> properties = new Dictionary<string, dynamic>(StringComparer.CurrentCultureIgnoreCase);

        public DynamicElement(XElement element) {
            this.element = element;
        }

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

        private Dictionary<string, dynamic> LoadAllProperties() {
            if (this.allPropertiesLoaded)
                return this.properties;

            if (this.element.HasElements) {
                var uniqueElements = this.element.Elements().Where(el => this.element.Elements().Count(el2 => el2.Name.LocalName.Equals(el.Name.LocalName)) == 1);
                var repeatedElements = this.element.Elements().Except(uniqueElements);

                foreach (var repeatedElementGroup in repeatedElements.GroupBy(re => re.Name.LocalName).OrderBy(el => el.Key)) {
                    if (!this.properties.ContainsKey(repeatedElementGroup.Key)) {
                        var list = new List<dynamic>();

                        foreach (var repeatedElement in repeatedElementGroup)
                            list.Add((repeatedElement.HasAttributes || repeatedElement.HasElements) ? new DynamicElement(repeatedElement) : TypedValue(repeatedElement.Value));

                        this.properties[repeatedElementGroup.Key] = list;
                    }
                }

                foreach (var uniqueElement in uniqueElements)
                    if (!this.properties.ContainsKey(uniqueElement.Name.LocalName))
                        this.properties[uniqueElement.Name.LocalName] = (uniqueElement.HasAttributes || uniqueElement.HasElements) ? new DynamicElement(uniqueElement) : TypedValue(uniqueElement.Value);
            }

            // Add attributes, if any
            if (element.Attributes().Any())
                foreach (var attribute in element.Attributes())
                    if (!attribute.IsNamespaceDeclaration && !this.properties.ContainsKey(attribute.Name.LocalName))
                        this.properties[attribute.Name.LocalName] = TypedValue(attribute.Value);

            this.allPropertiesLoaded = true;

            return this.properties;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            if (!this.properties.ContainsKey(binder.Name)) {
                bool haveMatch = false;

                // first check to see if the binder.name is an element
                if (this.element.HasElements) {
                    var match = this.element.Elements().Where(el => el.Name.LocalName.Equals(binder.Name, StringComparison.CurrentCultureIgnoreCase)).ToList();

                    if (match.Count > 0) {
                        haveMatch = true;

                        if (match.Count == 1) {
                            if (match[0].HasElements || match[0].HasAttributes)
                                this.properties[binder.Name] = new DynamicElement(match[0]);
                            else
                                this.properties[binder.Name] = TypedValue(match[0].Value);
                        } else {
                            var list = new List<DynamicElement>();

                            foreach (var element in match)
                                list.Add((element.HasAttributes || element.HasElements) ? new DynamicElement(element) : TypedValue(element.Value));

                            this.properties[binder.Name] = list;
                        }
                    }
                }

                // second if not an element then is it an attribute
                if (!haveMatch && this.element.HasAttributes) {
                    var match = this.element.Attributes().Where(el => el.Name.LocalName.Equals(binder.Name, StringComparison.CurrentCultureIgnoreCase)).ToList();

                    if (match.Count > 0) {
                        haveMatch = true;

                        this.properties[binder.Name] = TypedValue(match[0].Value);
                    }
                }
            }

            if (!this.properties.TryGetValue(binder.Name, out result))
                result = 0;

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {
            this.properties[binder.Name] = value as dynamic;
            return true;
        }


        public void Add(string key, dynamic value) {
            throw new InvalidOperationException();
        }

        public bool ContainsKey(string key) {
            return LoadAllProperties().ContainsKey(key);
        }

        public ICollection<string> Keys {
            get { return LoadAllProperties().Keys; }
        }

        public bool Remove(string key) {
            throw new InvalidOperationException();
        }

        public bool TryGetValue(string key, out dynamic value) {
            return LoadAllProperties().TryGetValue(key, out value);
        }

        public ICollection<dynamic> Values {
            get { return LoadAllProperties().Values; }
        }

        public dynamic this[string key] {
            get { return this.LoadAllProperties()[key]; }
            set { throw new InvalidOperationException(); }
        }

        public void Add(KeyValuePair<string, dynamic> item) {
            throw new InvalidOperationException();
        }

        public void Clear() {
            throw new InvalidOperationException();
        }

        public bool Contains(KeyValuePair<string, dynamic> item) {
            return LoadAllProperties().Contains(item);
        }

        public void CopyTo(KeyValuePair<string, dynamic>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public int Count {
            get { return LoadAllProperties().Count; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        public bool Remove(KeyValuePair<string, dynamic> item) {
            throw new InvalidOperationException();
        }

        public IEnumerator<KeyValuePair<string, dynamic>> GetEnumerator() {
            return LoadAllProperties().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return LoadAllProperties().GetEnumerator();
        }
    }

    static public class XElementExtensions
    {
        static public dynamic TypedValue(string value) {
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

        /// <summary>
        /// Returns a readonly dynamic interpretation of the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        static public dynamic ToDynamicElement(this XElement element) {
            return new DynamicElement(element);
        }

        /// <summary>
        /// Returns a dynamic interpretation of the specified element. This can be somewhat slow on large
        /// nested documents. For a faster readonly implementation refer to the ToDynamicElement extension method.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
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

                    properties.Add(repeatedElementGroup.Key, list);
                }

                foreach (var uniqueElement in uniqueElements)
                    properties.Add(uniqueElement.Name.LocalName, ToDynamic(uniqueElement));
            }

            // Add attributes, if any
            if (element.Attributes().Any())
                foreach (var attribute in element.Attributes())
                    if (!attribute.IsNamespaceDeclaration)
                        properties.Add(attribute.Name.LocalName, TypedValue(attribute.Value));

            if (!element.HasElements)
                properties.Add(element.Name.LocalName, element.Value);

            if (element.HasElements || element.HasAttributes)
                return item;

            return TypedValue(element.Value);
        }

        /// <summary>
        /// Returns the first named element found in the specified element.
        /// Search is performed in named order.
        /// </summary>
        /// <param name="element">The element to search in.</param>
        /// <param name="name">The name(s) to search for.</param>
        /// <returns></returns>
        static public XElement Element(this XElement element, params string[] name) {
            return element.Elements()
                .Where((x) => x.Name.LocalName.IsIn(name))
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the named elements found in the specified element.
        /// </summary>
        /// <param name="element">The element to search in.</param>
        /// <param name="name">The name(s) to search for.</param>
        /// <returns></returns>
        static public IEnumerable<XElement> Elements(this XElement element, params string[] name) {
            return element.Elements()
                .Where((x) => x.Name.LocalName.IsIn(name));
        }
    }
}
