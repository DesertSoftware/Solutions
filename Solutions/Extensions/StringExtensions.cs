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
    /// <summary>
    /// Depends on <see cref="DesertSoftware.Solutons.Extensions.CharExtensions"/>
    /// </summary>
    static public class StringExtensions
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

        /// <summary>
        /// returns true if any one of the values equals the string using the specified comparison function to determine equality
        /// </summary>
        /// <param name="s"></param>
        /// <param name="comparison"></param>
        /// <returns>
        ///   <c>true</c> if the specified comparison is in; otherwise, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings 
        /// ignoring case.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool IsPrefixedWith(this string s, params string[] values) {
            return IsPrefixedWith(s, StringComparison.CurrentCultureIgnoreCase, values);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings when
        /// compared using the specified comparison option.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="comparison"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool IsPrefixedWith(this string s, StringComparison comparison, params string[] values) {
            if (s == null || values == null)
                return false;

            foreach (var value in values)
                if (s.StartsWith(value, comparison))
                    return true;

            return false;
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings ignoring
        /// case and returns the specified string that matched. An empty string is returned if none of the 
        /// specified strings were found to be a match.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public string PrefixOf(this string s, params string[] values) {
            return PrefixOf(s, StringComparison.CurrentCultureIgnoreCase, values);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings when
        /// compared using the specified comparison option and returns the specified string that matched. An
        /// empty string is returned if none of the specified strings were found to be a match.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="comparison"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public string PrefixOf(this string s, StringComparison comparison, params string[] values) {
            if (s == null || values == null)
                return "";

            foreach (var value in values)
                if (s.StartsWith(value, comparison))
                    return value;

            return "";
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings 
        /// ignoring case.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool IsSuffixedWith(this string s, params string[] values) {
            return IsSuffixedWith(s, StringComparison.CurrentCultureIgnoreCase, values);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings when
        /// compared using the specified comparison option.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="comparison"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public bool IsSuffixedWith(this string s, StringComparison comparison, params string[] values) {
            if (s == null || values == null)
                return false;

            foreach (var value in values)
                if (s.EndsWith(value, comparison))
                    return true;

            return false;
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings ignoring
        /// case and returns the specified string that matched. An empty string is returned if none of the 
        /// specified strings were found to be a match.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public string SuffixOf(this string s, params string[] values) {
            return SuffixOf(s, StringComparison.CurrentCultureIgnoreCase, values);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings when
        /// compared using the specified comparison option and returns the specified string that matched. An
        /// empty string is returned if none of the specified strings were found to be a match.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="comparison"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public string SuffixOf(this string s, StringComparison comparison, string[] values) {
            if (s == null || values == null)
                return "";

            foreach(var value in values) 
                if (s.EndsWith(value, comparison))
                    return value;

            return "";
        }

        /// <summary>
        /// Returns an array of type T of a comma seperated string of values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        static public T[] ToArray<T>(this string s) {
            List<T> values = new List<T>();

            foreach (var value in (s ?? "").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                values.Add((T)Convert.ChangeType(value, typeof(T)));

            return values.ToArray();
        }

        /// <summary>
        /// Returns the first set of numeric characters found in the string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public string ToNumeric(this string s) {
            char prev = ' ';        // lookback character
            bool scanning = true;   // true until end of string or first digit found
            StringBuilder sb = new StringBuilder();

            // harvest first set of numeric digits with periods and commas from the string
            foreach (var ch in (s ?? "")) {
                if (scanning && ch.In("1234567890"))
                    scanning = false;

                if (!scanning && ch.In("1234567890.,")) {
                    // if number is signed, append the sign first
                    if (prev == '-')
                        sb.Append(prev);

                    sb.Append(ch);
                } else
                    break;

                prev = ch;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the specified string is a whole or decimal numeric value.
        /// </summary>
        /// <param name="s">The string to evaluate.</param>
        /// <returns></returns>
        static public bool IsNumeric(this string s) {
            double value;

            return double.TryParse(s, out value);
        }

        /// <summary>
        /// Returns the titlecase (ProperCase) value of the string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        static public string ToTitleCase(this string s) {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
        }

        /// <summary>
        /// Returns the specified string with all non-printable ASCII characters removed.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public string ToAscii(this string s) {
            using (System.IO.StringWriter writer = new System.IO.StringWriter()) {
                foreach (char c in s)
                    if (c >= '\x20' && c <= '\x7e')
                        writer.Write(c);

                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns the specified string with all markup tags (such as "<html></html>") removed.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public string TrimMarkup(this string s) {
            return System.Text.RegularExpressions.Regex.Replace(s ?? "", @"<.*?>", "");
        }

        /// <summary>
        /// Replaces the format item in the string with the string representation of a corresponding object parameter.
        /// </summary>
        /// <param name="s">The string format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        static public string Format(this string s, params object[] args) {
            return string.Format(s, args);
        }
    }
}
