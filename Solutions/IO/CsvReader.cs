//
//  Copyright 2012, Desert Software Solutions Inc.
//    CsvReader.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/IO/CsvReader.cs
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
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

        public void Close() {
            if (this.reader != null)
                try {
                    this.reader.Close();
                } catch { }
        }

        /// <summary>
        /// Reads the line.
        /// </summary>
        /// <returns></returns>
        public dynamic ReadLine() {
            string line = this.reader.ReadLine();

            if (line == null)
                return null;    // end of file

            // simplistic parser
            // shortcomings of this implementation; quoted commas are not ignored
            string[] values = line.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            var result = new ExpandoObject();
            var resultProperties = result as IDictionary<string, object>;

            if (this.header != null && this.header.Length > 0) {
                for (var index = 0; index < this.header.Length; index++)
                    if (index < values.Length)
                        resultProperties[this.header[index]] = values[index].TypedValue();

                return result;
            }

            for (var index = 0; index < values.Length; index++)
                resultProperties[string.Format("_{0}", index)] = values[index].TypedValue();

            return result;
        }

        public IEnumerable<dynamic> ReadAllLines() {
            dynamic line;

            while ((line = ReadLine()) != null)
                yield return line;
        }

        public IEnumerable<dynamic> ReadAllLines(Func<dynamic, bool> selector) {
            dynamic line;

            while ((line = ReadLine()) != null)
                if (selector(line))
                    yield return line;
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
        static internal string[] Trim(this string[] strings) {
            if (strings == null)
                return null;

            for (var index = 0; index < strings.Length; index++)
                strings[index] = strings[index].Trim();

            return strings;
        }

        static internal dynamic TypedValue(this string value) {
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

            if (DateTime.TryParse(value, out datetimeValue))
                return datetimeValue;

            return value.Trim().Trim('"');  // trim whitespace and then trim leading/trailing quotes
        }
    }
}
