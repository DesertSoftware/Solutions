//
//  Copyright 2014, Desert Software Solutions Inc.
//    DataSource.cs: https://gist.github.com/rostreim/9453953
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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using DesertSoftware.Solutions.Extensions;

namespace DesertSoftware.Solutions.Data
{
    public class DataSource
    {
        public class ConnectionDefinition
        {
            public string ConnectionString { get; set; }
            public string ProviderName { get; set; }
        }

        public class DataDefinition
        {
            public ConnectionDefinition Connection { get; set; }
            public string Query { get; set; }
        }

        static private IDictionary<string, dynamic> GetDictionary(dynamic d) {
            if (d == null)
                return null;

            IDictionary<string, dynamic> dictionary = d as IDictionary<string, dynamic>;

            if (dictionary == null) {
                dictionary = new Dictionary<string, dynamic>();

                foreach (var property in d.GetType().GetProperties())
                    dictionary[property.Name] = property.GetValue(d, null);
            }

            return dictionary;
        }

        static private string GetParameterizedString(string s, IDictionary<string, dynamic> parameters) {
            StringBuilder data = new StringBuilder(s.Trim());

            // bind paraemeter values to place holder values in the query
            // placeholder values are defined as $(PARAMETERNAME)
            if (parameters != null) {
                // replace all correct case occurrences
                foreach (var key in parameters.Keys)
                    data.Replace(string.Format("@({0})", key), parameters[key].ToString());

                // replace all lowercase parameter expressions
                foreach (var key in parameters.Keys)
                    data.Replace(string.Format("@({0})", key.ToLower()), parameters[key].ToString());

                // replace all uppercase parameter expressions
                foreach (var key in parameters.Keys)
                    data.Replace(string.Format("@({0})", key.ToUpper()), parameters[key].ToString());
            }

            return data.ToString();
        }

        static private dynamic CompositeListOf(List<dynamic> data, string sourceColumn) {
            var list = new ExpandoObject();
            var listItems = list as IDictionary<string, object>;

            foreach (var item in data) {
                var values = item as IDictionary<string, dynamic>;

                if (values.ContainsKey(sourceColumn))
                    listItems[values[sourceColumn]] = item;
            }

            return list;
        }

        static public dynamic QueryDB(DataDefinition dataContext, dynamic parameters) {
            string[] attrs = dataContext.Connection.ProviderName.Split("+".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var db = new Database(dataContext.Connection.ConnectionString, provider: attrs[0]);

            // can't use db.First on dynamic; when exercising firstordefault on a dynamic type null references are thrown
            List<dynamic> result = db.ListOf<dynamic>(
                sql: dataContext.Query,
                parameters: GetDictionary(parameters),
                connectionTimeout: 0);

            // determine if we have a provider modifier (eg. System.Data.SqlClient+CompositListOf.name)
            if (attrs.Length > 1) {
                string[] projection = attrs[1].Split(".".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                if (projection.Length > 1)
                    switch (projection[0].ToLower()) {
                        case "compositelistof":
                            return CompositeListOf(result, projection[1]);
                    }
            }

            // default behavior is to return the first row or null if no rows selected
            return result.Count > 0 ? result[0] : null;
        }

        static public dynamic QueryInlineXml(DataDefinition dataContext, dynamic parameters = null) {
            if (dataContext == null) throw new ArgumentNullException("dataContext");
            if (string.IsNullOrEmpty(dataContext.Query)) throw new ArgumentNullException("dataContext.Query");
            if (string.IsNullOrEmpty(dataContext.Query.Trim())) throw new ArgumentNullException("dataContext.Query");

            string xml = GetParameterizedString(dataContext.Query, GetDictionary(parameters));

            return XElement
                .Parse(xml)
                .ToDynamicElement(); //.ToDynamic();
        }

        static public dynamic QueryXml(DataDefinition dataContext, dynamic parameters = null) {
            if (dataContext == null) throw new ArgumentNullException("dataContext");
            if (dataContext.Connection == null) throw new ArgumentNullException("dataContext.Connection");

            XElement xmlData = null;
            XmlNamespaceManager namespaceManager = null;

            if (dataContext.Connection.ConnectionString.StartsWith("file://", StringComparison.CurrentCultureIgnoreCase)) {
                string filename = GetParameterizedString(dataContext.Connection.ConnectionString.Substring("file://".Length), GetDictionary(parameters));

                using (var fileReader = System.IO.File.OpenRead(filename)) {
                    using (XmlReader reader = XmlReader.Create(fileReader)) {
                        xmlData = XElement.Load(reader);
                        namespaceManager = new XmlNamespaceManager(reader.NameTable);
                    }
                }
            }

            if (dataContext.Connection.ConnectionString.StartsWith("file=", StringComparison.CurrentCultureIgnoreCase)) {
                string filename = GetParameterizedString(dataContext.Connection.ConnectionString.Substring("file=".Length), GetDictionary(parameters));

                using (var fileReader = System.IO.File.OpenRead(filename)) {
                    using (XmlReader reader = XmlReader.Create(fileReader)) {
                        xmlData = XElement.Load(reader);
                        namespaceManager = new XmlNamespaceManager(reader.NameTable);
                    }
                }
            }

            if (dataContext.Connection.ConnectionString.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) ||
                dataContext.Connection.ConnectionString.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)) {

                string url = GetParameterizedString(dataContext.Connection.ConnectionString, GetDictionary(parameters));

                WebRequest rq = HttpWebRequest.Create(url);

                using (var response = rq.GetResponse()) {
                    using (var stream = response.GetResponseStream()) {
                        using (XmlReader reader = XmlReader.Create(stream)) {
                            xmlData = XElement.Load(reader);
                            namespaceManager = new XmlNamespaceManager(reader.NameTable);
                        }
                    }
                }
            }

            if (xmlData != null && !string.IsNullOrEmpty(dataContext.Query.Trim())) {
                // bind parameters to query
                string query = GetParameterizedString(dataContext.Query, GetDictionary(parameters));

                if (query.StartsWith("<")) {
                    XElement xmQuery = XElement.Parse(query);

                    // assume that all attributes declared in this element refer to namespaces
                    foreach (var xmlns in xmQuery.Attributes())
                        namespaceManager.AddNamespace(xmlns.Name.LocalName, xmlns.Value);

                    query = xmQuery.Value.Trim();
                }

                xmlData = xmlData.XPathSelectElement(query, namespaceManager);
            }

            return xmlData == null
                ? null
                : xmlData.ToDynamicElement(); //.ToDynamic();
        }

        static public dynamic Query(DataDefinition dataContext, dynamic parameters = null) {
            if (dataContext == null) throw new ArgumentNullException("dataContext");
            if (dataContext.Connection == null) throw new ArgumentNullException("dataContext.Connection");
            if (string.IsNullOrEmpty(dataContext.Query)) throw new ArgumentNullException("dataContext.Query");

            string provider = dataContext.Connection.ProviderName ?? "System.Data.SqlClient";

            // case Inline.Data.Xml
            if (provider.StartsWith("inline.data.xml", StringComparison.CurrentCultureIgnoreCase))
                return QueryInlineXml(dataContext, parameters);

            // case "Inline.Data.Csv
            if (provider.StartsWith("inline.data.csv", StringComparison.CurrentCultureIgnoreCase))
                return null;

            // case Inline.Data.Json
            if (provider.StartsWith("inline.data.json", StringComparison.CurrentCultureIgnoreCase))
                return null;

            // case Inline.Data.Delimited
            if (provider.StartsWith("inline.data.delimited", StringComparison.CurrentCultureIgnoreCase))
                return null;

            // case System.Data.XmlClient
            if (provider.StartsWith("system.data.xmlclient", StringComparison.CurrentCultureIgnoreCase))
                return QueryXml(dataContext, parameters);

            // default System.Data.SqlClient
            return QueryDB(dataContext, parameters);
        }
    }
}
