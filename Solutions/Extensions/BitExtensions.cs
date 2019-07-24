//
//  Copyright 2013, Desert Software Solutions Inc.
//    BitExtensions.cs: 
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
    /// <summary>
    /// The BitExtensions class contains a set of extension methods that provide functionality to
    /// test whther a bit number is set and to set a bit number to an on or off value
    /// </summary>
    static public class BitExtensions
    {
        /// <summary>
        /// Returns whether the specified bit number is set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <returns></returns>
        static public bool Bit(this byte value, int bitNumber) {
            return ((value >> bitNumber) & 0x01) == 0x01;
        }

        /// <summary>
        /// Returns whether the specified bit number is set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <returns></returns>
        static public bool Bit(this ushort value, int bitNumber) {
            return ((value >> bitNumber) & 0x01) == 0x01;
        }

        /// <summary>
        /// Returns whether the specified bit number is set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <returns></returns>
        static public bool Bit(this short value, int bitNumber) {
            return ((value >> bitNumber) & 0x01) == 0x01;
        }

        /// <summary>
        /// Returns whether the specified bit number is set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <returns></returns>
        static public bool Bit(this UInt32 value, int bitNumber) {
            return ((value >> bitNumber) & 0x01) == 0x01;
        }

        /// <summary>
        /// Returns whether the specified bit number is set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <returns></returns>
        static public bool Bit(this Int32 value, int bitNumber) {
            return ((value >> bitNumber) & 0x01) == 0x01;
        }

        /// <summary>
        /// Returns whether the specified bit number is set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <returns></returns>
        static public bool Bit(this UInt64 value, int bitNumber) {
            return ((value >> bitNumber) & 0x01) == 0x01;
        }

        /// <summary>
        /// Returns whether the specified bit number is set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <returns></returns>
        static public bool Bit(this Int64 value, int bitNumber) {
            return ((value >> bitNumber) & 0x01) == 0x01;
        }

        /// <summary>
        /// Sets the specified bit number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <param name="on">if set to <c>true</c> [on].</param>
        /// <returns></returns>
        static public byte SetBit(this byte value, int bitNumber, bool on = true) {
            return on
                ? (byte)(value | ((byte)0x01 << bitNumber))
                : (byte)(value & ~((byte)0x01 << bitNumber));
        }

        /// <summary>
        /// Sets the specified bit number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <param name="on">if set to <c>true</c> [on].</param>
        /// <returns></returns>
        static public ushort SetBit(this ushort value, int bitNumber, bool on = true) {
            return on
                ? (ushort)(value | ((ushort)0x01 << bitNumber))
                : (ushort)(value & ~((ushort)0x01 << bitNumber));
        }

        /// <summary>
        /// Sets the specified bit number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <param name="on">if set to <c>true</c> [on].</param>
        /// <returns></returns>
        static public short SetBit(this short value, int bitNumber, bool on = true) {
            return on
                ? (short)((ushort)value | ((ushort)0x01 << bitNumber))
                : (short)((ushort)value & ~((ushort)0x01 << bitNumber));
        }

        /// <summary>
        /// Sets the specified bit number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <param name="on">if set to <c>true</c> [on].</param>
        /// <returns></returns>
        static public UInt32 SetBit(this UInt32 value, int bitNumber, bool on = true) {
            return on
                ? (UInt32)(value | ((UInt32)0x01 << bitNumber))
                : (UInt32)(value & ~((UInt32)0x01 << bitNumber));
        }

        /// <summary>
        /// Sets the specified bit number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <param name="on">if set to <c>true</c> [on].</param>
        /// <returns></returns>
        static public Int32 SetBit(this Int32 value, int bitNumber, bool on = true) {
            return on
                ? (Int32)((UInt32)value | ((UInt32)0x01 << bitNumber))
                : (Int32)((UInt32)value & ~((UInt32)0x01 << bitNumber));
        }

        /// <summary>
        /// Sets the specified bit number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <param name="on">if set to <c>true</c> [on].</param>
        /// <returns></returns>
        static public UInt64 SetBit(this UInt64 value, int bitNumber, bool on = true) {
            return on
                ? (UInt64)(value | ((UInt64)0x01 << bitNumber))
                : (UInt64)(value & ~((UInt64)0x01 << bitNumber));
        }

        /// <summary>
        /// Sets the specified bit number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bitNumber">The bit number.</param>
        /// <param name="on">if set to <c>true</c> [on].</param>
        /// <returns></returns>
        static public Int64 SetBit(this Int64 value, int bitNumber, bool on = true) {
            return on
                ? (Int64)((UInt64)value | ((UInt64)0x01 << bitNumber))
                : (Int64)((UInt64)value & ~((UInt64)0x01 << bitNumber));
        }
    }
}
