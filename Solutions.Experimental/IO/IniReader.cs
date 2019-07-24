//
//  Copyright 2012, Desert Software Solutions Inc.
//    IniReader.cs: 
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DesertSoftware.Solutions.Windows.API;

namespace Solutions.Experimental.IO
{
    /// <summary>
    /// The IniReader class reads keys from an INI file.
    /// </summary>
    /// <remarks>
    /// This class uses several Win32 API functions to read from and write to INI files. It will not work on Linux or FreeBSD.
    /// </remarks>
    public class IniReader
    {
        /// the full path to the INI file.
        private string m_Filename;
        
        /// Holds the active section name
        private string m_Section;

        /// The maximum number of bytes in a section buffer.
        private const int MAX_ENTRY = 32768;

        /// <summary>
        /// Initializes a new instance of the <see cref="IniReader"/> class.
        /// </summary>
        /// <param name="file">Specifies the full path to the INI file.</param>
        public IniReader(string file) {
            Filename = file;
        }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string Filename {
            get { return m_Filename; }
            set { m_Filename = value; }
        }

        /// <summary>Gets or sets the section you're working in. (aka 'the active section')</summary>
        /// <value>A String representing the section you're working in.</value>
        public string Section {
            get { return m_Section; }
            set { m_Section = value; }
        }

        /// <summary>Reads an Integer from the specified key of the specified section.</summary>
        /// <param name="section">The section to search in.</param>
        /// <param name="key">The key from which to return the value.</param>
        /// <param name="defVal">The value to return if the specified key isn't found.</param>
        /// <returns>Returns the value of the specified section/key pair, or returns the default value if the specified section/key pair isn't found in the INI file.</returns>
        public int ReadInteger(string section, string key, int defVal) {
            return Kernel32.GetPrivateProfileInt(section, key, defVal, Filename);
        }
        
        /// <summary>Reads an Integer from the specified key of the specified section.</summary>
        /// <param name="section">The section to search in.</param>
        /// <param name="key">The key from which to return the value.</param>
        /// <returns>Returns the value of the specified section/key pair, or returns 0 if the specified section/key pair isn't found in the INI file.</returns>
        public int ReadInteger(string section, string key) {
            return ReadInteger(section, key, 0);
        }
        
        /// <summary>Reads an Integer from the specified key of the active section.</summary>
        /// <param name="key">The key from which to return the value.</param>
        /// <param name="defVal">The section to search in.</param>
        /// <returns>Returns the value of the specified Key, or returns the default value if the specified Key isn't found in the active section of the INI file.</returns>
        public int ReadInteger(string key, int defVal) {
            return ReadInteger(Section, key, defVal);
        }
        
        /// <summary>Reads an Integer from the specified key of the active section.</summary>
        /// <param name="key">The key from which to return the value.</param>
        /// <returns>Returns the value of the specified key, or returns 0 if the specified key isn't found in the active section of the INI file.</returns>
        public int ReadInteger(string key) {
            return ReadInteger(key, 0);
        }
        
        /// <summary>Reads a String from the specified key of the specified section.</summary>
        /// <param name="section">The section to search in.</param>
        /// <param name="key">The key from which to return the value.</param>
        /// <param name="defVal">The value to return if the specified key isn't found.</param>
        /// <returns>Returns the value of the specified section/key pair, or returns the default value if the specified section/key pair isn't found in the INI file.</returns>
        public string ReadString(string section, string key, string defVal) {
            StringBuilder sb = new StringBuilder(MAX_ENTRY);
            int Ret = Kernel32.GetPrivateProfileString(section, key, defVal, sb, MAX_ENTRY, Filename);
            return sb.ToString();
        }
        
        /// <summary>Reads a String from the specified key of the specified section.</summary>
        /// <param name="section">The section to search in.</param>
        /// <param name="key">The key from which to return the value.</param>
        /// <returns>Returns the value of the specified section/key pair, or returns an empty String if the specified section/key pair isn't found in the INI file.</returns>
        public string ReadString(string section, string key) {
            return ReadString(section, key, "");
        }
        
        /// <summary>Reads a String from the specified key of the active section.</summary>
        /// <param name="key">The key from which to return the value.</param>
        /// <returns>Returns the value of the specified key, or returns an empty String if the specified key isn't found in the active section of the INI file.</returns>
        public string ReadString(string key) {
            return ReadString(Section, key);
        }
        
        /// <summary>Reads a Long from the specified key of the specified section.</summary>
        /// <param name="section">The section to search in.</param>
        /// <param name="key">The key from which to return the value.</param>
        /// <param name="defVal">The value to return if the specified key isn't found.</param>
        /// <returns>Returns the value of the specified section/key pair, or returns the default value if the specified section/key pair isn't found in the INI file.</returns>
        public long ReadLong(string section, string key, long defVal) {
            return long.Parse(ReadString(section, key, defVal.ToString()));
        }
        
        /// <summary>Reads a Long from the specified key of the specified section.</summary>
        /// <param name="section">The section to search in.</param>
        /// <param name="key">The key from which to return the value.</param>
        /// <returns>Returns the value of the specified section/key pair, or returns 0 if the specified section/key pair isn't found in the INI file.</returns>
        public long ReadLong(string section, string key) {
            return ReadLong(section, key, 0);
        }
        
        /// <summary>Reads a Long from the specified key of the active section.</summary>
        /// <param name="key">The key from which to return the value.</param>
        /// <param name="defVal">The section to search in.</param>
        /// <returns>Returns the value of the specified key, or returns the default value if the specified key isn't found in the active section of the INI file.</returns>
        public long ReadLong(string key, long defVal) {
            return ReadLong(Section, key, defVal);
        }
        
        /// <summary>Reads a Long from the specified key of the active section.</summary>
        /// <param name="key">The key from which to return the value.</param>
        /// <returns>Returns the value of the specified Key, or returns 0 if the specified Key isn't found in the active section of the INI file.</returns>
        public long ReadLong(string key) {
            return ReadLong(key, 0);
        }
        
        /// <summary>Reads a Byte array from the specified key of the specified section.</summary>
        /// <param name="section">The section to search in.</param>
        /// <param name="key">The key from which to return the value.</param>
        /// <returns>Returns the value of the specified section/key pair, or returns null (Nothing in VB.NET) if the specified section/key pair isn't found in the INI file.</returns>
        public byte[] ReadByteArray(string section, string key) {
            try {
                return Convert.FromBase64String(ReadString(section, key));
            } catch { }
            return null;
        }
        
        /// <summary>Reads a Byte array from the specified key of the active section.</summary>
        /// <param name="key">The key from which to return the value.</param>
        /// <returns>Returns the value of the specified key, or returns null (Nothing in VB.NET) if the specified key pair isn't found in the active section of the INI file.</returns>
        public byte[] ReadByteArray(string key) {
            return ReadByteArray(Section, key);
        }
        
        /// <summary>Reads a Boolean from the specified key of the specified section.</summary>
        /// <param name="section">The section to search in.</param>
        /// <param name="key">The key from which to return the value.</param>
        /// <param name="defVal">The value to return if the specified key isn't found.</param>
        /// <returns>Returns the value of the specified section/key pair, or returns the default value if the specified section/key pair isn't found in the INI file.</returns>
        public bool ReadBoolean(string section, string key, bool defVal) {
            return Boolean.Parse(ReadString(section, key, defVal.ToString()));
        }
        
        /// <summary>Reads a Boolean from the specified key of the specified section.</summary>
        /// <param name="section">The section to search in.</param>
        /// <param name="key">The key from which to return the value.</param>
        /// <returns>Returns the value of the specified section/key pair, or returns false if the specified section/key pair isn't found in the INI file.</returns>
        public bool ReadBoolean(string section, string key) {
            return ReadBoolean(section, key, false);
        }
        
        /// <summary>Reads a Boolean from the specified key of the specified section.</summary>
        /// <param name="key">The key from which to return the value.</param>
        /// <param name="defVal">The value to return if the specified key isn't found.</param>
        /// <returns>Returns the value of the specified key pair, or returns the default value if the specified key isn't found in the active section of the INI file.</returns>
        public bool ReadBoolean(string key, bool defVal) {
            return ReadBoolean(Section, key, defVal);
        }
        
        /// <summary>Reads a Boolean from the specified key of the specified section.</summary>
        /// <param name="key">The key from which to return the value.</param>
        /// <returns>Returns the value of the specified key, or returns false if the specified key isn't found in the active section of the INI file.</returns>
        public bool ReadBoolean(string key) {
            return ReadBoolean(Section, key);
        }
        
        /// <summary>Writes an Integer to the specified key in the specified section.</summary>
        /// <param name="section">The section to write in.</param>
        /// <param name="key">The key to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, int value) {
            return Write(section, key, value.ToString());
        }
        
        /// <summary>Writes an Integer to the specified key in the active section.</summary>
        /// <param name="key">The key to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string key, int value) {
            return Write(Section, key, value);
        }
        
        /// <summary>Writes a String to the specified key in the specified section.</summary>
        /// <param name="section">Specifies the section to write in.</param>
        /// <param name="key">Specifies the key to write to.</param>
        /// <param name="value">Specifies the value to write.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, string value) {
            return (Kernel32.WritePrivateProfileString(section, key, value, Filename) != 0);
        }
        
        /// <summary>Writes a String to the specified key in the active section.</summary>
        ///	<param name="key">The key to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string key, string value) {
            return Write(Section, key, value);
        }
        
        /// <summary>Writes a Long to the specified key in the specified section.</summary>
        /// <param name="section">The section to write in.</param>
        /// <param name="key">The key to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, long value) {
            return Write(section, key, value.ToString());
        }
        
        /// <summary>Writes a Long to the specified key in the active section.</summary>
        /// <param name="key">The key to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string key, long value) {
            return Write(Section, key, value);
        }
        
        /// <summary>Writes a Byte array to the specified key in the specified section.</summary>
        /// <param name="section">The section to write in.</param>
        /// <param name="key">The key to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, byte[] value) {
            if (value == null)
                return Write(section, key, (string)null);
            else
                return Write(section, key, value, 0, value.Length);
        }
        
        /// <summary>Writes a Byte array to the specified key in the active section.</summary>
        /// <param name="key">The key to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string key, byte[] value) {
            return Write(Section, key, value);
        }
        
        /// <summary>Writes a Byte array to the specified key in the specified section.</summary>
        /// <param name="section">The section to write in.</param>
        /// <param name="key">The key to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="offset">An offset in <i>value</i>.</param>
        /// <param name="length">The number of elements of <i>value</i> to convert.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, byte[] value, int offset, int length) {
            if (value == null)
                return Write(section, key, (string)null);
            else
                return Write(section, key, Convert.ToBase64String(value, offset, length));
        }
        
        /// <summary>Writes a Boolean to the specified key in the specified section.</summary>
        /// <param name="section">The section to write in.</param>
        /// <param name="key">The key to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, bool value) {
            return Write(section, key, value.ToString());
        }
        
        /// <summary>Writes a Boolean to the specified key in the active section.</summary>
        /// <param name="key">The key to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string key, bool value) {
            return Write(Section, key, value);
        }
        
        /// <summary>Deletes a key from the specified section.</summary>
        /// <param name="section">The section to delete from.</param>
        /// <param name="key">The key to delete.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool DeleteKey(string section, string key) {
            return (Kernel32.WritePrivateProfileString(section, key, null, Filename) != 0);
        }
        
        /// <summary>Deletes a key from the active section.</summary>
        /// <param name="key">The key to delete.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool DeleteKey(string key) {
            return (Kernel32.WritePrivateProfileString(Section, key, null, Filename) != 0);
        }
        
        /// <summary>Deletes a section from an INI file.</summary>
        /// <param name="section">The section to delete.</param>
        /// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool DeleteSection(string section) {
            return Kernel32.WritePrivateProfileSection(section, null, Filename) != 0;
        }
        
        /// <summary>Retrieves a list of all available sections in the INI file.</summary>
        /// <returns>Returns an ArrayList with all available sections.</returns>
        public ArrayList GetSectionNames() {
            try {
                byte[] buffer = new byte[MAX_ENTRY];
                Kernel32.GetPrivateProfileSectionNames(buffer, MAX_ENTRY, Filename);
                string[] parts = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');
                return new ArrayList(parts);
            } catch { }
            return null;
        }
    }
}