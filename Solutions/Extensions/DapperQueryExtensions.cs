//
//  Copyright 2014, Desert Software Solutions Inc.
//    DapperQueryExtensions.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Extensions/DapperQueryExtensions.cs
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
    /// The DapperQueryExtensions class provides functionality to handle set queries
    /// as in "select * from table where tableid in @values"
    /// Due to SQL parameter sniffing, the manner in which dapper expands this notation is
    /// not quite suitable when predeclaring the parameter inputs
    /// </summary>
    static public class DapperQueryExtensions
    {
        /// <summary>
        /// Returns a block declaration list of strings suitable for inclusion into a query.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="source">The source.</param>
        /// <param name="decl">The declaration variable prefix.</param>
        /// <returns></returns>
        static public string ToDeclaration(this string[] list, string source, string decl) {
            System.IO.StringWriter writer = new System.IO.StringWriter();

            if (list == null)
                return "";

            for (int i = 0; i < list.Length; i++)
                writer.WriteLine("declare @{0}{2} nvarchar(4000) = @{1}{2}", decl, source, i + 1);

            // add a reference to the source list so dapper will expand the reference
            // dapper doesn't care if the reference is contained in a comment 
            if (list.Length > 0)
                writer.WriteLine("--declare @{1}_x nvarchar(4000) = @{0}", source, decl);

            return writer.GetStringBuilder().ToString();
        }

        /// <summary>
        /// Returns a block declaration list of integers suitable for inclusion into a query.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="source">The source.</param>
        /// <param name="decl">The declaration variable prefix.</param>
        /// <returns></returns>
        static public string ToDeclaration(this int[] list, string source, string decl) {
            System.IO.StringWriter writer = new System.IO.StringWriter();

            if (list == null)
                return "";

            for (int i = 0; i < list.Length; i++)
                writer.WriteLine("declare @{0}{2} int = @{1}{2}", decl, source, i + 1);

            // add a reference to the source list so dapper will expand the reference
            // dapper doesn't care if the reference is contained in a comment 
            if (list.Length > 0)
                writer.WriteLine("--declare @{1}_x nvarchar(4000) = @{0}", source, decl);

            return writer.GetStringBuilder().ToString();
        }

        // format a string representing a list of values suitable for inclusion in an 'IN' statement.
        static private string MakeSetNotation(int length, string decl) {
            System.IO.StringWriter writer = new System.IO.StringWriter();

            for (int i = 0; i < length; i++)
                writer.Write(",@{0}{1}", decl, i + 1);

            return writer.GetStringBuilder().ToString().TrimStart(',');
        }

        /// <summary>
        /// Returns a formatted string representing the list of string values suitable for inclusion in an 'IN' statement.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="decl">The declaration variable prefix.</param>
        /// <returns></returns>
        static public string ToSetNotation(this string[] list, string decl) {
            return list != null
                ? MakeSetNotation(list.Length, decl)
                : "";
        }

        /// <summary>
        /// Returns a formatted string representing the list of integer values suitable for inclusion in an 'IN' statement.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="decl">The declaration variable prefix.</param>
        /// <returns></returns>
        static public string ToSetNotation(this int[] list, string decl) {
            return list != null
                ? MakeSetNotation(list.Length, decl)
                : "";
        }
    }
}
