﻿//
//  Copyright 2012, 2019 Desert Software Solutions Inc.
//    CsvReader.cs: 
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
using System.IO;
using System.Linq;
using System.Text;

namespace DesertSoftware.Solutions.IO
{
    public class CsvReader : IDisposable
    {
        public const int NoHeader = -1;

        private TextReader reader;
        private string[] header;

        class Options
        {
            // determines if the detectdatetime option is present in the specified array of options
            static public bool DetectDateTime(params string[] options) {
                foreach (var option in options)
                    if (option.Trim() == "detectdatetime")
                        return true;

                return false;
            }

            // determines if the detectdatetime option is present in the specified comma seperated options string
            static public bool DetectDateTime(string options) {
                // options is represented as a comma seperated list of content specific options
                return DetectDateTime((options ?? "").ToLower().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public class CsvValue
        {
            public bool IsQuoted { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="firstLineOfData">The first line of data.</param>
        /// <param name="header">The header.</param>
        public CsvReader(Stream stream, int firstLineOfData, params string[] header) {
            this.reader = new StreamReader(stream);
            this.header = header;
            Initialize(-1, firstLineOfData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="header">The header.</param>
        public CsvReader(Stream stream, params string[] header) {
            this.reader = new StreamReader(stream);

            if (header != null && header.Length > 0)
                this.header = header;
            else
                Initialize(0, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="headerLineNumber">The header line number.</param>
        /// <param name="firstLineOfData">The first line of data.</param>
        public CsvReader(Stream stream, int headerLineNumber = 0, int firstLineOfData = 0) {
            this.reader = new StreamReader(stream);
            Initialize(headerLineNumber, firstLineOfData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="firstLineOfData">The first line of data.</param>
        /// <param name="header">The header.</param>
        public CsvReader(TextReader reader, int firstLineOfData, params string[] header) {
            this.reader = reader;
            this.header = header;
            Initialize(-1, firstLineOfData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="header">The header.</param>
        public CsvReader(TextReader reader, params string[] header) {
            this.reader = reader;

            if (header != null && header.Length > 0)
                this.header = header;
            else
                Initialize(0, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="headerLineNumber">The header line number.</param>
        /// <param name="firstLineOfData">The first line of data.</param>
        public CsvReader(TextReader reader, int headerLineNumber = 0, int firstLineOfData = 0) {
            this.reader = reader;
            Initialize(headerLineNumber, firstLineOfData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="firstLineOfData">The first line of data.</param>
        /// <param name="header">The header.</param>
        public CsvReader(string filepath, int firstLineOfData, params string[] header) {
            this.reader = new StreamReader(File.OpenRead(filepath));
            this.header = header;
            Initialize(-1, firstLineOfData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="header">The header.</param>
        public CsvReader(string filepath, params string[] header) {
            this.reader = new StreamReader(File.OpenRead(filepath));

            if (header != null && header.Length > 0)
                this.header = header;
            else
                Initialize(0, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="headerLineNumber">The header line number.</param>
        /// <param name="firstLineOfData">The first line of data.</param>
        public CsvReader(string filepath, int headerLineNumber = 0, int firstLineOfData = 0) {
            this.reader = new StreamReader(File.OpenRead(filepath));
            Initialize(headerLineNumber, firstLineOfData);
        }

        private void Initialize(int headerLineNumber, int firstLineOfData) {
            // find the header declaration if specified (headerLineNumber > -1)
            if (headerLineNumber >= 0) {
                int currentLineNumber = -1;
                string line = this.reader.ReadLine();

                while (++currentLineNumber < headerLineNumber && (line = this.reader.ReadLine()) != null) ;

                if (line != null)
                    this.header = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Trim();
            }

            // scan forward to the first line of data (firstLineOfData > 0)
            if (firstLineOfData > 0) {
                int currentLineNumber = 0;

                while (currentLineNumber < firstLineOfData && this.reader.ReadLine() != null)
                    currentLineNumber++;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close() {
            if (this.reader != null)
                try {
                    this.reader.Close();
                } catch { }
        }

        public string[] Columns {
            get { return this.header; }
        }

        // read next fully quoted line from the reader. The number of physical lines
        // may be more than one since it is possible we may encounter a crlf within
        // a quoted column value. The final result is one string of text.
        private string ReadLine() {
            bool done = false;
            bool inQuote = false;
            bool escapedQuote = false;
            string line = this.reader.ReadLine();
            List<string> lines = new List<string>();

            if (line == null)
                return null;

            lines.Add(line);

            while (!done) {
                foreach (var character in line.Select((val, index) => new { val, index })) {
                    if (character.val == '"') {
                        //If we've hit a quote character...

                        if (character.val == '"' && inQuote) {
                            // Does it appear to be a closing quote?
                            // """ ensure we don't attempt to index beyond the length of the line
                            if (line[Math.Min(character.index + 1, line.Length - 1)] == character.val && character.index + 1 < line.Length && !escapedQuote) {
                                // If the character afterwards is also a quote, this is to escape that (not a closing quote).
                                // Flag that we are escaped for the next character. Don't add the escaping quote.
                                escapedQuote = true;
                            } else
                                if (escapedQuote) {
                                    //This is an escaped quote. Add it and revert quoteIsEscaped to false.
                                    escapedQuote = false;
                                } else {
                                    inQuote = false;
                                }
                        } else {
                            if (!inQuote) {
                                inQuote = true;
                            } else {
                                // value.Append(character.val); //...It's a quote inside a quote.
                            }
                        }
                    }
                    // This, is a, "test"", followed, "some crlf line breakes in a line"
                }

                done = !inQuote;
                // are we in a quote? if so,
                // do we need to combine the next line with this line? (done will be false)
                if (!done) {
                    line = this.reader.ReadLine();

                    // file ended early??
                    if (line == null)
                        done = true;

                    if (!done)
                        lines.Add(line);
                }
            }

            return string.Join("\n", lines.ToArray());
        }

        /// <summary>
        /// Reads the next line and returns an object consisting of the values contained in the line.
        /// </summary>
        /// <returns></returns>
        public dynamic ReadLine(bool detectDateTimeValues = true) {
            string line = ReadLine();

            if (line == null)
                return null;    // end of file

            // parse the line into an array of values
            CsvValue[] values = line.EnumerateCsvValues().ToArray();

            var result = new ExpandoObject();
            var resultProperties = result as IDictionary<string, object>;

            if (this.header != null && this.header.Length > 0) {
                for (var index = 0; index < this.header.Length; index++)
                    if (index < values.Length)
                        resultProperties[this.header[index]] = values[index].TypedValue(detectDateTimeValues);

                return result;
            }

            // no header exists, add each value as "_ColumnPos" eg. "_0"
            for (var index = 0; index < values.Length; index++)
                resultProperties[string.Format("_{0}", index)] = values[index].TypedValue();

            return result;
        }

        /// <summary>
        /// Reads all lines.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<dynamic> ReadAllLines(bool detectDateTimeValues = true) {
            dynamic line;

            while ((line = ReadLine(detectDateTimeValues)) != null)
                yield return line;
        }

        /// <summary>
        /// Reads all lines.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public IEnumerable<dynamic> ReadAllLines(Func<dynamic, bool> selector, bool detectDateTimeValues = true) {
            dynamic line;

            while ((line = ReadLine(detectDateTimeValues)) != null)
                if (selector(line))
                    yield return line;
        }

        /// <summary>
        /// Reads all lines.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public IEnumerable<dynamic> ReadAllLines(string options) {
            return ReadAllLines(Options.DetectDateTime(options));
        }

        /// <summary>
        /// Resolves a column index to a column name as defined in the header or anonymous.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;Index value must be in the range of 0 to number of columns.</exception>
        public string ColumnName(int index) {
            if (index < 0 || (this.header != null && index >= this.header.Length))
                throw new ArgumentOutOfRangeException("index", "Index value must be in the range of 0 to number of columns.");

            return this.header != null
                ? this.header[index]
                : string.Format("_{0}", index);
        }

        /// <summary>
        /// Gets the value of the column for the specified index.
        /// </summary>
        /// <param name="values">The values as returned from a ReadLine operation.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">values</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index;Index value must be in the range of 0 to number of columns.</exception>
        public dynamic ColumnValue(dynamic values, int index) {
            if (values == null) throw new ArgumentNullException("values");
            if (index < 0 || (this.header != null && index >= this.header.Length))
                throw new ArgumentOutOfRangeException("index", "Index value must be in the range of 0 to number of columns.");

            IDictionary<string, dynamic> dictionary = values as IDictionary<string, dynamic>;

            if (dictionary == null) {
                dictionary = new Dictionary<string, dynamic>();

                foreach (var property in values.GetType().GetProperties())
                    try {
                        dictionary[property.Name] = property.GetValue(values, null);
                    } catch { }
            }

            if (dictionary != null) {
                if (this.header != null) {
                    return dictionary.ContainsKey(this.header[index])
                        ? dictionary[this.header[index]]
                        : null;
                }

                string name = string.Format("_{0}", index);
                return dictionary.ContainsKey(name)
                    ? dictionary[name]
                    : null;
            }

            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (this.reader != null)
                try {
                    this.reader.Close();
                } catch { }
        }
    }

    static internal class CsvReaderExtensions
    {
        /// <summary>
        /// Trims the specified strings.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <returns></returns>
        static internal string[] Trim(this string[] strings) {
            if (strings == null)
                return null;

            for (var index = 0; index < strings.Length; index++)
                strings[index] = strings[index].Trim();

            return strings;
        }

        static internal string Right(this string s, int length) {
            return (s ?? "").Substring(Math.Max(s.Length - length, 0), Math.Min(length, s.Length));
        }

        static internal string Left(this string s, int length) {
            return (s ?? "").Substring(0, Math.Min(length, s.Length));
        }

        /// <summary>
        /// Determines a value type suitable to represent the specified string value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="detectDateTime">if set to <c>true</c> [detect date time].</param>
        /// <returns></returns>
        static internal dynamic TypedValue(this CsvReader.CsvValue value, bool detectDateTime = true) {
            // treat quoted values as literal string
            if (value.IsQuoted) 
                return value.Value;

            return TypedValue(value.Value, detectDateTime);
        }

        /// <summary>
        /// Determines a value type suitable to contain the specified string value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        static internal dynamic TypedValue(this string value, bool detectDateTime = true) {
            // attempt to determine the basic value type
            int intValue = 0;
            double doubleValue = 0;
            decimal decimalValue = 0;
            float floatValue = 0;
            DateTime datetimeValue = DateTime.MinValue;

            if (int.TryParse(value, out intValue))
                return intValue;

            if (double.TryParse(value, out doubleValue))
                return doubleValue;

            if (decimal.TryParse(value, out decimalValue))
                return decimalValue;

            if (float.TryParse(value, out floatValue))
                return floatValue;

            // dd-MMM-yyyy
            if (detectDateTime) {
                if (DateTime.TryParse(value.Replace("-", " "), out datetimeValue))
                    return datetimeValue;

                string[] tokens = value.Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length > 2 &&
                    DateTime.TryParse(("0" + tokens[0]).Right(2) + " " + tokens[1] + " " + ("20" + tokens[2]).Right(4), out datetimeValue))
                    return datetimeValue;
            }

            return value.Trim().Trim('"');  // trim whitespace and then trim leading/trailing quotes
        }

        static private bool In(this char ch, params char[] values) {
            foreach (char value in values)
                if (ch == value)
                    return true;

            return false;
        }

        /// <summary>
        /// Enumerates the CSV values contained in the specified string.
        /// </summary>
        /// <param name="s">The string to enumerate from.</param>
        /// <param name="delimiters">One or more characters that delimit a value. (default ',')</param>
        /// <returns></returns>
        static internal IEnumerable<CsvReader.CsvValue> EnumerateCsvValues(this string s, params char[] delimiters) {

            var inQuote = false;
            var isQuoted = false;
            var escapedQuote = false;
            var value = new StringBuilder();

            // ensure we have a default delimiter to work with if no delimiters specified
            delimiters = delimiters.Length > 0 ? delimiters : new char[] { ',' };

            // start of a new value begins with a delimiter, so append one for the parser to mark end of line.
            s = string.Format("{0}{1}", s, delimiters.Length > 0 ? delimiters[0] : ',');

            // scanner: scan the line looking for delimiters and quotes. everything else is a value character
            foreach (var character in s.Select((val, index) => new { val, index })) {
                if (character.val.In(delimiters)) {

                    // Have a delimiter character. 
                    // We've potentially reached the start of a new value.
                    // Determine if we are in quotes or not. If not in quotes, return the characters scanned up to this point.

                    if (!inQuote) {
                        yield return new CsvReader.CsvValue() { IsQuoted = isQuoted, Value = value.ToString().Trim() };
                        value.Clear();
                        isQuoted = false;
                    } else
                        value.Append(character.val);

                } else {
                    if (character.val != ' ') {
                        if (character.val == '"') {
                            //If we've hit a quote character...

                            if (character.val == '"' && inQuote) {
                                //Does it appear to be a closing quote?
                                if (s[character.index + 1] == character.val && !escapedQuote) {
                                    // If the character afterwards is also a quote, this is to escape that (not a closing quote).
                                    // Flag that we are escaped for the next character. Don't add the escaping quote.
                                    escapedQuote = true;
                                } else
                                    if (escapedQuote) {
                                        //This is an escaped quote. Add it and revert quoteIsEscaped to false.
                                        escapedQuote = false;
                                        value.Append(character.val);
                                    } else {
                                        inQuote = false;
                                    }
                            } else {
                                if (!inQuote) {
                                    inQuote = true;
                                    isQuoted = true;
                                } else {
                                    value.Append(character.val); //...It's a quote inside a quote.
                                }
                            }
                        } else {
                            value.Append(character.val);
                        }
                    } else {
                        if (!string.IsNullOrWhiteSpace(value.ToString())) {
                            //Append only if not new cell
                            value.Append(character.val);
                        }
                    }
                }
            }

        }
    }
}