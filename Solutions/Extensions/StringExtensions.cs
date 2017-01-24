//
//  Copyright 2013, Desert Software Solutions Inc.
//    StringExtensions.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Extensions/StringExtensions.cs
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
    public static class StringExtensions
    {
        /// <summary>
        /// Removes all leading and trailing white space character from each string in the array
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        static public string[] Trim(this string[] strings) {
            if (strings != null)
                for (int index = 0; index < strings.Length; index++)
                    strings[index] = strings[index] != null ? strings[index].Trim() : strings[index];

            return strings;
        }

        /// <summary>
        /// Returns true if all of the values are contained in the string (ignoring case)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool HasAll(this string s, params string[] values) {
            return HasAll(s, StringComparison.CurrentCultureIgnoreCase, values);
        }

        /// <summary>
        /// Returns true if all of the values are contained in the string using the specified equality comparison
        /// </summary>
        /// <param name="s"></param>
        /// <param name="comparison">accepted values (CurrentCultureIgnoreCase, CurrentCulture)</param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool HasAll(this string s, StringComparison comparison, params string[] values) {
            if (s == null || values == null)
                return false;

            string lowerCase = comparison == StringComparison.CurrentCultureIgnoreCase ? s.ToLower() : s;

            foreach (var value in values)
                if (!lowerCase.Contains(comparison == StringComparison.CurrentCultureIgnoreCase ? value.ToLower() : value))
                    return false;

            return true;
        }

        /// <summary>
        /// Returns true if at least one of the values are contained in the string (ignoring case)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool Has(this string s, params string[] values) {
            return Has(s, StringComparison.CurrentCultureIgnoreCase, values);
        }

        /// <summary>
        /// Returns true if at least one of the values are contained in the string using the specified equaility comparison
        /// </summary>
        /// <param name="s"></param>
        /// <param name="comparison">accepted values (CurrentCultureIgnoreCase, CurrentCulture)</param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool Has(this string s, StringComparison comparison, params string[] values) {
            if (s == null || values == null)
                return false;

            string lowerCase = comparison == StringComparison.CurrentCultureIgnoreCase ? s.ToLower() : s;

            foreach (var value in values)
                if (lowerCase.Contains(comparison == StringComparison.CurrentCultureIgnoreCase ? value.ToLower() : value))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns true if any one of the values equals the string (ignoring case)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool IsIn(this string s, params string[] values) {
            return IsIn(s, StringComparison.CurrentCultureIgnoreCase, values);
        }

        /// <summary>
        /// returns true if any one of the values equals the string via the specified equality comparison
        /// </summary>
        /// <param name="s"></param>
        /// <param name="comparison"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool IsIn(this string s, StringComparison comparison, params string[] values) {
            if (s == null || values == null)
                return false;

            foreach (var value in values)
                if (s.Equals(value, comparison))
                    return true;

            return false;
        }

        static public bool IsIn(this string s, Func<string, bool> comparison) {
            if (s == null || comparison == null)
                return false;

            return comparison(s);
        }

        static public bool IsNotIn(this string s, Func<string, bool> comparison) {
            if (s == null || comparison == null)
                return false;

            return comparison(s);
        }


        static public bool Has(this string s, Func<string, bool> comparison) {
            if (s == null || comparison == null)
                return false;

            return comparison(s);
        }

        /// <summary>
        /// Returns true if none of the values equals the string (ignoring case)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool IsNotIn(this string s, params string[] values) {
            return IsNotIn(s, StringComparison.CurrentCultureIgnoreCase, values);
        }

        /// <summary>
        /// Returns true if none of the values equals the string via the specified equality comparison
        /// </summary>
        /// <param name="s"></param>
        /// <param name="comparison"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool IsNotIn(this string s, StringComparison comparison, params string[] values) {
            if (s == null || values == null)
                return false;

            foreach (var value in values)
                if (s.Equals(value, comparison))
                    return false;

            return true;
        }

    }
}
