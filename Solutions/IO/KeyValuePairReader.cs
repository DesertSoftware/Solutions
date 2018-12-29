//
//  Copyright 2012, Desert Software Solutions Inc.
//    KeyValuePairReader.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/IO/KeyValuePairReader.cs
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
    public class KeyValuePairReader : IDisposable
    {
        private TextReader reader;
        private int lineNumber = -1;
        private bool disposed = false;
        private char[] delimiters = new char[] { '=' };
        private string[] comments = new string[] { "#" };

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public KeyValuePairReader(TextReader reader) {
            this.reader = reader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairReader"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public KeyValuePairReader(string filename) {
            this.reader = new StreamReader(filename);
        }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        public string[] Comments {
            get { return this.comments; }
            set { this.comments = value ?? new string[] { }; }
        }

        /// <summary>
        /// Gets or sets the delimiters.
        /// </summary>
        /// <value>
        /// The delimiters.
        /// </value>
        public char[] Delimiters {
            get { return this.delimiters; }
            set { this.delimiters = value ?? new char[] { }; }
        }

        private bool IsEmpty(string s) {
            return string.IsNullOrWhiteSpace(s);
        }

        private bool IsCommented(string s) {
            if (this.comments == null)
                return false;

            s = (s ?? "").Trim();
            foreach (var phrase in this.comments)
                if (s.StartsWith(phrase))
                    return true;

            return false;
        }

        private bool IsNotWellFomed(string s) {
            return (s ?? "").IndexOfAny(this.delimiters) <= 0;
        }

        // returns a keyvalue pair instance from the contents of s. if s is not in tag <delimiter> text
        // format then the key value of the keyvalue pair instance returned will be null/default of TKey.
        // the string variable is expected to be formatted as tag <delimiter> text (Keyname = value text)
        private KeyValuePair<TKey, TValue> ParseKeyValuePair<TKey, TValue>(string s) {
            int valueIndex = s.IndexOfAny(this.delimiters);

            if (valueIndex <= 0)
                return new KeyValuePair<TKey, TValue>(default(TKey), (TValue)Convert.ChangeType(s, typeof(TValue)));

            return new KeyValuePair<TKey, TValue>(
                (TKey)Convert.ChangeType(s.Substring(0, valueIndex).Trim(), typeof(TKey)),
                (TValue)Convert.ChangeType(s.Substring(valueIndex + 1, s.Length - (valueIndex + 1)).Trim(), typeof(TValue)));
        }

        /// <summary>
        /// Reads the next key value. Null is returned if no further key value pairs are available.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns></returns>
        public KeyValuePair<TKey, TValue>? Read<TKey, TValue>() {
            string s = "";
            bool scanning = true;

            if (this.reader == null)
                return null;

            // scan to next line defining a well formed key value pair
            while (scanning && (s = this.reader.ReadLine()) != null) {
                this.lineNumber++;

                scanning =
                    IsEmpty(s) ||           // ignore empty lines
                    IsCommented(s) ||       // ignore commented lines
                    IsNotWellFomed(s);      // ignore lines not containing a delimiter
            }

            if (s == null) return null;

            return ParseKeyValuePair<TKey, TValue>(s);
        }

        /// <summary>
        /// Enumerates the key value pairs.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> EnumerateKeyValuePairs<TKey, TValue>() {
            KeyValuePair<TKey, TValue>? entry;

            while ((entry = Read<TKey, TValue>()) != null)
                yield return entry.Value;
        }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public int LineNumber { get { return this.lineNumber; } }

        /// <summary>
        /// Closes the reader.
        /// </summary>
        public void Close() {
            if (this.reader != null)
                this.reader.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    try {
                        Close();
                    } catch {
                        // ignore any exceptions we may generate
                    }
                }

                // Indicate that the instance has been disposed.
                this.disposed = true;
            }
        }
    }
}
