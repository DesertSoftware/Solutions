//
//  Copyright 2013, 2017 Desert Software Solutions Inc.
//    NumberExtensions.cs: 
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
using System.Linq;
using System.Text;

namespace DesertSoftware.Solutions.Extensions
{
    static public class NumberExtensions
    {
        /// <summary>
        /// Returns a sequence of numbers up to the specified maximum.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        static public int[] Sequence(this int min, int max, int step = 1) {
            var values = new List<int>();

            for (var value = min; value <= max; value += step)
                values.Add(value);

            return values.ToArray();
        }

        /// <summary>
        /// Returns a default value when the specified single value is indeterminate (NaN, Inifinity, etc).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        static public Single DefaultWhenIndeterminate(this Single value, Single defaultValue = 0) {
            if ((System.Single.IsNaN(value)) || (System.Single.IsInfinity(value)) || (value <= System.Single.Epsilon))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// Returns a default value when the specified single value is indeterminate (NaN, Inifinity, etc).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        static public double DefaultWhenIndeterminate(this double value, double defaultValue = 0) {
            if ((System.Double.IsNaN(value)) || (System.Double.IsInfinity(value)) || (value <= System.Double.Epsilon))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// Returns <c>true</c> if the value is between the specified start and end values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="inclusive">if set to <c>true</c> [inclusive].</param>
        /// <returns></returns>
        static public bool Between(this double value, int start, int end, bool inclusive = true) {
            if (inclusive)
                return value >= start && value <= end;

            return value > start && value < end;
        }

        /// <summary>
        /// Returns <c>true</c> if the value is between the specified start and end values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="inclusive">if set to <c>true</c> [inclusive].</param>
        /// <returns></returns>
        static public bool Between(this int value, int start, int end, bool inclusive = true) {
            if (inclusive)
                return value >= start && value <= end;

            return value > start && value < end;
        }

        /// <summary>
        /// Returns <c>true</c> if the value is between the specified start and end values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="inclusive">if set to <c>true</c> [inclusive].</param>
        /// <returns></returns>
        static public bool Between(this long value, long start, long end, bool inclusive = true) {
            if (inclusive)
                return value >= start && value <= end;

            return value > start && value < end;
        }

        /// <summary>
        /// Returns <c>true</c> if the value is found to exist in the specified set of values
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        static public bool In(this int value, params int[] values) {
            foreach (var v in values)
                if (value == v)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns the numeric value of a binary string encoded from right to left (bit 0 is rightmost character).
        /// </summary>
        /// <param name="s">The binary string.</param>
        /// <returns></returns>
        static public T BinaryNumber<T>(this string s) {
            ulong result = 0;
            ulong powerOfTwo = 0;

            // ensure we are dealing with non null string with a maximum length of 64 characters (bits)
            s = (s ?? "").Length > 64 ? s.Substring(0, 64) : (s ?? "");

            // convert right to left bit encoded string.
            foreach (char ch in s.Reverse()) {
                powerOfTwo = powerOfTwo != 0 ? powerOfTwo * 2 : 1;
                result = powerOfTwo * (ulong)Math.Min(Convert.ToInt32((ch == ' ' ? '0' : ch).ToString()), 1);  // ensure we only deal with 0 and 1 (interpret spaces as zero)
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }

        /// <summary>
        /// Returns a single precision floating point numeric value from four bytes in the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="startIndex">The start index. (default 0)</param>
        /// <param name="length">The number of bytes to convert. (default and max value is four)</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        static public Single ToSingle(this byte[] bytes, int startIndex = 0, int length = 4) {
            if (bytes == null)
                return 0;

            if (startIndex >= bytes.Length) throw new ArgumentOutOfRangeException("startIndex");
            byte[] value = new byte[4] { 0, 0, 0, 0 };
            
            // verify we have enough source bytes to fulfill the length (maximum of four bytes)
            length = Math.Min((bytes.Length - startIndex) - length >= length ? length : bytes.Length - startIndex, 4);

            // copy the requested byte values to be converted into the value array right justified
            Array.Copy(bytes, startIndex, value, 4 - length, length);
            return System.BitConverter.ToSingle(value, 0).DefaultWhenIndeterminate(0);
        }

        /// <summary>
        /// Returns a 32 bit unsigned integer from four bytes in the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The number of bytes to convert. (default and max value is four)</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        static public UInt32 ToUInt32(this byte[] bytes, int startIndex = 0, int length = 4) {
            if (bytes == null)
                return 0;

            if (startIndex >= bytes.Length) throw new ArgumentOutOfRangeException("startIndex");
            byte[] value = new byte[4] { 0, 0, 0, 0 };

            // verify we have enough source bytes to fulfill the length (maximum of four bytes)
            length = Math.Min((bytes.Length - startIndex) - length >= length ? length : bytes.Length - startIndex, 4);

            // copy the requested byte values to be converted into the value array right justified
            Array.Copy(bytes, startIndex, value, 4 - length, length);
            return System.BitConverter.ToUInt32(value, 0);
        }

        /// <summary>
        /// Returns a 32 bit signed integer from four bytes in the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The number of bytes to convert. (default and max value is four)</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        static public Int32 ToInt32(this byte[] bytes, int startIndex = 0, int length = 4) {
            if (bytes == null)
                return 0;

            if (startIndex >= bytes.Length) throw new ArgumentOutOfRangeException("startIndex");
            byte[] value = new byte[4] { 0, 0, 0, 0 };

            // verify we have enough source bytes to fulfill the length (maximum of four bytes)
            length = Math.Min((bytes.Length - startIndex) - length >= length ? length : bytes.Length - startIndex, 4);

            // copy the requested byte values to be converted into the value array right justified
            Array.Copy(bytes, startIndex, value, 4 - length, length);
            return System.BitConverter.ToInt32(value, 0);
        }

        /// <summary>
        /// Returns a 16 bit signed integer from two bytes in the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The number of bytes to convert. (default and max value is two)</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        static public Int16 ToInt16(this byte[] bytes, int startIndex = 0, int length = 2) {
            if (bytes == null)
                return 0;

            if (startIndex >= bytes.Length) throw new ArgumentOutOfRangeException("startIndex");
            byte[] value = new byte[2] { 0, 0 };

            // verify we have enough source bytes to fulfill the length (maximum of two bytes)
            length = Math.Min((bytes.Length - startIndex) - length >= length ? length : bytes.Length - startIndex, 2);

            // copy the requested byte values to be converted into the value array right justified
            Array.Copy(bytes, startIndex, value, 2 - length, length);
            return System.BitConverter.ToInt16(value, 0);
        }

    }
}
