//
//  Copyright 2014, Desert Software Solutions Inc.
//    ValueBag.cs:
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

namespace DesertSoftware.Solutions.Dynamix
{
    /// <summary>
    /// Provides functionality to set and get values with a dynamic instance of the form "foo.bar"
    /// </summary>
    public class ValueBag
    {
        private dynamic source;
        private bool readOnlyDictionary = false;
        private IDictionary<string, dynamic> valueDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueBag"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public ValueBag(dynamic source) {
            this.source = source;
            this.valueDictionary = GetDictionary(source);
            this.readOnlyDictionary = (source as IDictionary<string, dynamic>) == null;
        }

        /// <summary>
        /// Returns an IDictionary interface of the specified value bag
        /// </summary>
        /// <param name="bag">The value bag</param>
        /// <returns></returns>
        static public IDictionary<string, dynamic> ToDictionary(ValueBag bag) {
            return bag.valueDictionary;
        }

        // returns the properties of an instance in dictionary form
        static private IDictionary<string, dynamic> GetDictionary(dynamic d) {
            if (d == null)
                return null;

            IDictionary<string, dynamic> dictionary = d as IDictionary<string, dynamic>;

            if (dictionary == null) {
                dictionary = new Dictionary<string, dynamic>();

                foreach (var property in d.GetType().GetProperties())
                    try {
                        dictionary[property.Name] = property.GetValue(d, null);
                    } catch { 
                        // eat any exceptions raised
                    }
            }

            return dictionary;
        }

        static private int IndexNumber(string token) {
            int value = -1;

            int.TryParse(token.Split('[')[1].Split(']')[0].Trim(), out value);
            return value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        static public dynamic GetValue(dynamic source, string fieldName) {
            int index = -1;
            IDictionary<string, dynamic> values = GetDictionary(source);
            string[] fields = fieldName.Trim().Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            while (++index < fields.Length - 1 && values != null)
                if (values != null)
                    try {
                        values = GetDictionary(values
                            .Where((kv) => kv.Key.Equals(fields[index], StringComparison.CurrentCultureIgnoreCase))
                            //                            .Select((kv) => { return kv.Value; })
                            .Select((kv) => { return fields[index].Contains("[") ? kv.Value[IndexNumber(fields[index])] : kv.Value; })
                            .FirstOrDefault());
                    } catch (ArgumentOutOfRangeException) {
                        // Console.WriteLine("Warning: '{0}' is outside the range of available items.", fields[index]);
                        return null;
                    } catch (Exception) {
                        // Console.WriteLine("Warning: referencing '{0}' generated the following error. '{1}'", fields[index], ex.Message);
                        return null;
                    }

            if (values == null)
                return null;

            try {
                // values[fields[index]] is case sensitive
                // search for the source value in a case insensitive fashion
                return values
                    .Where((kv) => kv.Key.Equals(fields[index], StringComparison.CurrentCultureIgnoreCase))
                    //                    .Select((kv) => { return kv.Value; })
                    .Select((kv) => { return fields[index].Contains("[") ? kv.Value[IndexNumber(fields[index])] : kv.Value; })
                    .FirstOrDefault();

            } catch (ArgumentOutOfRangeException) {
                // Console.WriteLine("Warning: index '{0}' is outside the range of available items in '{1}'. Expression '{2}' ignored.", IndexNumber(fields[index]), fields[index], expression);
            } catch (Exception) {
                // Console.WriteLine("Warning: referencing '{0}' generated the following error. '{1}'. Expression '2' ignored.", fields[index], ex.Message, expression);
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <value>
        /// The type of the source.
        /// </value>
        public Type SourceType {
            get {  return this.source.GetType(); }
        }

        /// <summary>
        /// Gets the name of the source type.
        /// </summary>
        /// <value>
        /// The name of the source type.
        /// </value>
        public string SourceTypeName {
            get { return this.source.GetType().Name; }
        }

        /// <summary>
        /// Gets or sets the value at the specified index.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <param name="index">The index/field name to get or set.</param>
        /// <returns></returns>
        public dynamic this[string index] {
            get { return GetValue(index); }
            set { SetValue(index, value); }
        }

        /// <summary>
        /// Determines whether the value bag contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        ///   <c>true</c> if the value bag contains the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(string key) {
            // get the fieldnames
            int index = -1;
            IDictionary<string, dynamic> values = this.valueDictionary;
            string[] fields = key.Trim().Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            while (++index < fields.Length - 1 && values != null)
                if (values != null)
                    try {
                        values = GetDictionary(values
                            .Where((kv) => kv.Key.Equals(fields[index], StringComparison.CurrentCultureIgnoreCase))
                            // accommodate indexed fields eg.  `items[0], Items[0].Details, etc`
                            .Select((kv) => { return fields[index].Contains("[") ? kv.Value[IndexNumber(fields[index])] : kv.Value; })
                            .FirstOrDefault());
                    } catch (ArgumentOutOfRangeException) {
                        // Console.WriteLine("Warning: '{0}' is outside the range of available items.", fields[index]);
                        return false;
                    } catch (Exception) {
                        return false;
                        // Console.WriteLine("Warning: referencing '{0}' generated the following error. '{1}'", fields[index], ex.Message);
                    }

            if (values == null)
                return false;

            try {
                // values[fields[index]] is case sensitive
                // search for the source value in a case insensitive fashion
                return values
                    .Where((kv) => kv.Key.Equals(fields[index], StringComparison.CurrentCultureIgnoreCase))
                    .Select((kv) => true)
                    .FirstOrDefault();

            } catch (ArgumentOutOfRangeException) {
                // Console.WriteLine("Warning: index '{0}' is outside the range of available items in '{1}'. Expression '{2}' ignored.", IndexNumber(fields[index]), fields[index], expression);
            } catch (Exception) {
                // Console.WriteLine("Warning: referencing '{0}' generated the following error. '{1}'. Expression '2' ignored.", fields[index], ex.Message, expression);
            }

            return false;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public dynamic GetValue(string fieldName) {
            // get the fieldnames
            int index = -1;
            IDictionary<string, dynamic> values = this.valueDictionary;
            string[] fields = fieldName.Trim().Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            while (++index < fields.Length - 1 && values != null)
                if (values != null)
                    try {
                        values = GetDictionary(values
                            .Where((kv) => kv.Key.Equals(fields[index], StringComparison.CurrentCultureIgnoreCase))
                            //                            .Select((kv) => { return kv.Value; })
                            .Select((kv) => { return fields[index].Contains("[") ? kv.Value[IndexNumber(fields[index])] : kv.Value; })
                            .FirstOrDefault());
                    } catch (ArgumentOutOfRangeException) {
                        // Console.WriteLine("Warning: '{0}' is outside the range of available items.", fields[index]);
                        return null;
                    } catch (Exception) {
                        // Console.WriteLine("Warning: referencing '{0}' generated the following error. '{1}'", fields[index], ex.Message);
                        return null;
                    }

            if (values == null)
                return null;

            try {
                // values[fields[index]] is case sensitive
                // search for the source value in a case insensitive fashion
                return values
                    .Where((kv) => kv.Key.Equals(fields[index], StringComparison.CurrentCultureIgnoreCase))
                    //                    .Select((kv) => { return kv.Value; })
                    .Select((kv) => { return fields[index].Contains("[") ? kv.Value[IndexNumber(fields[index])] : kv.Value; })
                    .FirstOrDefault();

            } catch (ArgumentOutOfRangeException) {
                // Console.WriteLine("Warning: index '{0}' is outside the range of available items in '{1}'. Expression '{2}' ignored.", IndexNumber(fields[index]), fields[index], expression);
            } catch (Exception) {
                // Console.WriteLine("Warning: referencing '{0}' generated the following error. '{1}'. Expression '2' ignored.", fields[index], ex.Message, expression);
            }

            return null;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        public void SetValue(string fieldName, dynamic value) {
            int index = -1;
            dynamic instance = this.source;
            IDictionary<string, dynamic> values = this.valueDictionary;
            string[] fields = fieldName.Trim().Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            while (++index < fields.Length - 1 && values != null)
                if (values != null)
                    try {
                        instance = values
                                    .Where((kv) => kv.Key.Equals(fields[index], StringComparison.CurrentCultureIgnoreCase))
                            //                            .Select((kv) => { return kv.Value; })
                                    .Select((kv) => { return fields[index].Contains("[") ? kv.Value[IndexNumber(fields[index])] : kv.Value; })
                                    .FirstOrDefault();

                        IDictionary<string, dynamic> newValues = GetDictionary(instance);

                        if (newValues == null) {
                            instance = new System.Dynamic.ExpandoObject();
                            values[fields[index]] = instance;
                            newValues = GetDictionary(values[fields[index]]);
                        }

                        values = newValues;
                    } catch (ArgumentOutOfRangeException) {
                        values = null;
                        // Console.WriteLine("Warning: '{0}' is outside the range of available items.", fields[index]);
                    } catch (Exception) {
                        values = null;
                        // Console.WriteLine("Warning: referencing '{0}' generated the following error. '{1}'", fields[index], ex.Message);
                    }

            if (values == null) return;

            try {
                // values[fields[index]] is case sensitive
                // search for the source value in a case insensitive fashion
                var sourceKey = values
                    .Where((kv) => kv.Key.Equals(fields[index], StringComparison.CurrentCultureIgnoreCase))
                    .Select((kv) => { return kv.Key; })
                    .FirstOrDefault();

                values[sourceKey ?? fields[index]] = value;
                //if (sourceKey != null)
                //    values[sourceKey] = value;
                //else
                //    values[fields[index]] = value;

                //                this.GetType().GetProperties()[0].PropertyType
                if (this.readOnlyDictionary) {
                    // get the property referenced and invoke the SetValue method on it
                    foreach (var property in this.source.GetType().GetProperties())
                        if (property.Name.Equals(sourceKey ?? fields[index], StringComparison.CurrentCultureIgnoreCase))
                            try {
                                property.SetValue(instance, Convert.ChangeType(value, property.PropertyType), null);
                                break;
                            } catch { }

                }

            } catch (ArgumentOutOfRangeException) {
                // Console.WriteLine("Warning: index '{0}' is outside the range of available items in '{1}'. Expression '{2}' ignored.", IndexNumber(fields[index]), fields[index], expression);
            } catch (Exception) {
                // Console.WriteLine("Warning: referencing '{0}' generated the following error. '{1}'. Expression '2' ignored.", fields[index], ex.Message, expression);
            }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        static public void SetValue(dynamic source, string fieldName, dynamic value) {
            int index = -1;
            IDictionary<string, dynamic> values = GetDictionary(source);
            string[] fields = fieldName.Trim().Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            while (++index < fields.Length - 1 && values != null)
                if (values != null)
                    try {
                        IDictionary<string, dynamic> newValues = GetDictionary(values
                                    .Where((kv) => kv.Key.Equals(fields[index], StringComparison.CurrentCultureIgnoreCase))
                                    .Select((kv) => { return kv.Value; })
                                    .FirstOrDefault());

                        if (newValues == null) {
                            values[fields[index]] = new System.Dynamic.ExpandoObject();
                            newValues = GetDictionary(values[fields[index]]);
                        }

                        values = newValues;
                    } catch (ArgumentOutOfRangeException) {
                        // Console.WriteLine("Warning: '{0}' is outside the range of available items.", fields[index]);
                        values = null;
                    } catch (Exception) {
                        // Console.WriteLine("Warning: referencing '{0}' generated the following error. '{1}'", fields[index], ex.Message);
                        values = null;
                    }

            if (values == null) return;

            try {
                // values[fields[index]] is case sensitive
                // search for the source value in a case insensitive fashion
                var sourceKey = values
                    .Where((kv) => kv.Key.Equals(fields[index], StringComparison.CurrentCultureIgnoreCase))
                    .Select((kv) => { return kv.Key; })
                    .FirstOrDefault();

                if (sourceKey != null)
                    values[sourceKey] = value;
                else
                    values[fields[index]] = value;

            } catch (ArgumentOutOfRangeException) {
                // Console.WriteLine("Warning: index '{0}' is outside the range of available items in '{1}'. Expression '{2}' ignored.", IndexNumber(fields[index]), fields[index], expression);
            } catch (Exception) {
                // Console.WriteLine("Warning: referencing '{0}' generated the following error. '{1}'. Expression '2' ignored.", fields[index], ex.Message, expression);
            }
        }
    }
}
