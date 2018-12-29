//
//  Copyright 2012, Desert Software Solutions Inc.
//    CsvReader.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/IO/CsvWriter.cs
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
using System.IO;
using System.Linq;
using System.Text;

namespace DesertSoftware.Solutions.IO
{
    public class CsvWriter : IDisposable
    {
        private TextWriter writer;
        private bool haveHeader = false;
        private bool haveRows = false;
        private int numberOfColumns = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public CsvWriter(Stream stream) {
            this.writer = new StreamWriter(stream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public CsvWriter(string path) {
            this.writer = new StreamWriter(File.Create(path));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public CsvWriter(TextWriter writer) {
            this.writer = writer;
        }

        /// <summary>
        /// Closes the current CsvWriter and the underlying stream.
        /// </summary>
        public void Close() {
            this.writer.Close();
        }

        /// <summary>
        /// Gets the <see cref="System.Text.Encoding"/> in which the output is writtem.
        /// </summary>
        /// <value>
        /// The encoding.
        /// </value>
        public Encoding Encoding {
            get { return this.writer.Encoding; }
        }

        /// <summary>
        /// Clears all buffers for the current CsvWriter and causes any buffered data to be written to the underlying stream.
        /// </summary>
        public void Flush() {
            this.writer.Flush();
        }

        /// <summary>
        /// Gets an object that controls formatting.
        /// </summary>
        /// <value>
        /// The format provider.
        /// </value>
        public IFormatProvider FormatProvider {
            get { return this.writer.FormatProvider; }
        }

        /// <summary>
        /// Gets or sets the line terminator string used by the current CsvWriter.
        /// </summary>
        /// <value>
        /// The new line.
        /// </value>
        public string NewLine {
            get { return this.writer.NewLine; }
            set { this.writer.NewLine = value; }
        }

        /// <summary>
        /// Writes a csv header.
        /// </summary>
        /// <param name="columnNames">The column names.</param>
        /// <exception cref="System.ArgumentException">At least one column name must be specified</exception>
        /// <exception cref="System.InvalidOperationException">
        /// Multiple headers are not permitted or
        /// Headers are not permitted after the first row
        /// </exception>
        public void WriteHeader(params string[] columnNames) {
            if (columnNames.Length == 0)
                throw new ArgumentException("columnNames", "At least one column name must be specified");

            if (haveHeader)
                throw new InvalidOperationException("Multiple headers are not permitted");

            if (haveRows)
                throw new InvalidOperationException("Headers are not permitted after the first row");

            this.haveHeader = true;
            this.numberOfColumns = columnNames.Length;
            this.writer.WriteLine(string.Join(",", columnNames));
        }

        /// <summary>
        /// Writes a csv row of data.
        /// </summary>
        /// <param name="rowData">The row data.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">rowData;The number of data items to write does not match previous writes</exception>
        public void WriteRow(params object[] rowData) {
            bool firstColumn = true;

            if ((this.haveHeader || this.haveRows) && rowData.Length != this.numberOfColumns)
                throw new ArgumentOutOfRangeException("rowData", "The number of data items to write does not match previous writes");

            foreach (var item in rowData) {
                if (!firstColumn)
                    this.writer.Write(",");

                // just escape all strings with double quotes to handle the 
                // possible presence of commas in the data. We also escape all
                // double quotes in the data as well
                if (item != null && item.GetType() == typeof(string))
                    this.writer.Write("\"{0}\"", item.ToString().Replace("\"", "\"\""));
                else
                    this.writer.Write(item);

                firstColumn = false;
                if (!this.haveHeader && !this.haveRows)
                    this.numberOfColumns++;
            }

            this.writer.WriteLine();

            this.haveRows = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            this.writer.Dispose();
        }
    }
}