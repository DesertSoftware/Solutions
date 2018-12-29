//
//  Copyright 2014, Desert Software Solutions Inc.
//    MD5Checksum.cs:
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Security/MD5Checksum.cs
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
using System.Security.Cryptography;
using System.Text;

namespace DesertSoftware.Solutions.Security
{
    static public class MD5Checksum
    {
        /// <summary>
        /// Computes the checksum.
        /// </summary>
        /// <param name="stream">The input to compute the checksum for.</param>
        /// <returns></returns>
        static public string ComputeChecksum(this Stream stream) {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in new MD5CryptoServiceProvider().ComputeHash(stream))
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        /// Computes the checksum.
        /// </summary>
        /// <param name="bytes">The bytes to compute the checksum for.</param>
        /// <returns></returns>
        static public string ComputeChecksum(this byte[] bytes) {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in new MD5CryptoServiceProvider().ComputeHash(bytes))
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        /// Computes the checksum.
        /// </summary>
        /// <param name="s">The string to compute the checksum for.</param>
        /// <returns></returns>
        static public string ComputeChecksum(this string s) {
            return ComputeChecksum(Encoding.UTF8.GetBytes(s));
        }
    }
}
