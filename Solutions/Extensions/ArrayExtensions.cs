//
//  Copyright 2013, 2019 Desert Software Solutions Inc.
//    ArrayExtensions.cs:
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
    /// Provides extended functions dealing with arrays
    /// </summary>
    static public class ArrayExtensions
    {
        /// <summary>
        /// Returns a sub array of elements from the source array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        static public T[] SubArray<T>(this T[] source, int index, int length) {
            T[] result = new T[length];

            Array.Copy(source, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Returns the index of the item in a sorted set of values matching the specified value.
        /// If the set of values contains more than one element equal to value, the index of only
        /// one of the occurrences is returned (not necessarily the first one). If the set of values
        /// to be searched is not in ascending sort order, the resulting index is indeterminate.
        /// The result of defaultIndex is returned when the specified value is not present.
        /// </summary>
        /// <param name="values">The values. (Values are expected to be in ascending sort order)</param>
        /// <param name="value">The value to search for.</param>
        /// <param name="defaultIndex">A Function to determine the index when value is not present.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">values</exception>
        static public int IndexOf(this DateTime[] values, DateTime value, Func<int, int> defaultIndex = null) {
            if (values == null) throw new ArgumentNullException("values");
                
            // The elements of the values array must already be sorted in increasing value according to the sort order 
            // defined by the IComparable<T> implementation; otherwise, the result is indeterminate.
            int index = Array.BinarySearch(values, value);

            // If values does not contain the specified value, the BinarySearch returns a negative integer. The bitwise 
            // complement operator (~) can be applied to the negative result to produce an index. If this index is equal to
            // the size of the values array, there are no elements larger than value in the array. Otherwise, it is the 
            // index of the first element that is larger than value.

            // If the index is negative, it represents the bitwise complement of the next larger element in the array.

            return index < 0
                ? defaultIndex != null ? defaultIndex(~index) : Math.Max(0, Math.Min((~index) - 1, values.Length - 1))
                : index;
        }

        /// <summary>
        /// Combines the specified array into the source array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        static public T[] Combine<T>(this T[] source, T[] array) {
            if (source == null) throw new ArgumentException("source");
            if (array == null) throw new ArgumentNullException("array");

            T[] result = new T[source.Length + array.Length];

            Array.Copy(source, 0, result, 0, source.Length);
            Array.Copy(array, 0, result, source.Length, array.Length);
            return result;
        }

        /// <summary>
        /// Rotates the items in the array leftward for the specified length.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// length;value must be a positive number greater than zero
        /// or
        /// length;value exceeds the number of items in the array
        /// </exception>
        static public T[] RotateLeft<T>(this T[] source, int length) {
            if (source == null) 
                return source;

            if (length < 1) throw new ArgumentOutOfRangeException("length", "value must be a positive number greater than zero");
            if (source.Length - length < 1) throw new ArgumentOutOfRangeException("length", "value exceeds the number of items in the array");

            T[] result = new T[source.Length];

            // given an array of [1, 2, 3, 4, 5, 6, 7, 8] and a rotate length of 2 the result should be [3, 4, 5, 6, 7, 8, 1, 2]
            // source.length = 8; length = 2; 
            
            Array.Copy(source, length, result, 0, source.Length - length);      // Array.Copy(source, 2, result, 0, 8 - 2 = 6)
            Array.Copy(source, 0, result, source.Length - length, length);      // Array.Copy(source, 0, result, 8 - 2 = 6, 2);

            return result;
        }

        /// <summary>
        /// Rotates the items in the array rightward for the specified length.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// length;value must be a positive number greater than zero
        /// or
        /// length;value exceeds the number of items in the array
        /// </exception>
        static public T[] RotateRight<T>(this T[] source, int length) {
            if (source == null)
                return source;

            if (length < 1) throw new ArgumentOutOfRangeException("length", "value must be a positive number greater than zero");
            if (source.Length - length < 1) throw new ArgumentOutOfRangeException("length", "value exceeds the number of items in the array");

            T[] result = new T[source.Length];

            // given an array of [1, 2, 3, 4, 5, 6, 7, 8] and a rotate length of 2 the result should be [7, 8, 1, 2, 3, 4, 5, 6]
            // source.length = 8; length = 2; 
            
            Array.Copy(source, source.Length - length, result, 0, 2);   // Array.Copy(source, 8 - 2 = 6, result, 0, 2)
            Array.Copy(source, 0, result, 2, source.Length - length);   // Array.Copy(source, 0, result, 2, 8 - 2 = 6)

            return result;
        }

        /// <summary>
        /// Returns the index of the maximum element value contained in the source array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <returns></returns>
        static public long MaxIndex<T>(this T[] source) where T : IComparable<T> {
            if (source == null || source.Length == 0)
                return 0;

            T currentValue = source[0];
            long maxIndex = 0;

            for (int index = 0; index < source.Length; index++)
                if (source[index].CompareTo(currentValue) > 0) {
                    currentValue = source[index];
                    maxIndex = index;
                }

            return maxIndex;
        }

        /// <summary>
        /// Returns the index of the maximum element value contained in the source array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="firstGreaterThanSecond">A function that accepts two values and determines if the first value is greater than the second.</param>
        /// <returns></returns>
        static public long MaxIndex<T>(this T[] source, Func<T, T, bool> firstGreaterThanSecond) where T : IComparable<T> {
            if (source == null || source.Length == 0)
                return 0;

            T currentValue = source[0];
            long maxIndex = 0;

            for (int index = 0; index < source.Length; index++)
                if (firstGreaterThanSecond(source[index], currentValue)) {
                    currentValue = source[index];
                    maxIndex = index;
                }

            return maxIndex;
        }

        /// <summary>
        /// Returns the index of the minimum element value contained in the source array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <returns></returns>
        static public long MinIndex<T>(this T[] source) where T : IComparable<T> {
            if (source == null || source.Length == 0)
                return 0;

            T currentValue = source[0];
            long minIndex = 0;

            for (long index = 0; index < source.Length; index++)
                if (source[index].CompareTo(currentValue) < 0) {
                    currentValue = source[index];
                    minIndex = index;
                }

            return minIndex;
        }

        /// <summary>
        /// Returns the index of the minimum element value contained in the source array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="firstLessThanSecond">A function that accepts two values and determines if the first value is less than the second.</param>
        /// <returns></returns>
        static public long MinIndex<T>(this T[] source, Func<T, T, bool> firstLessThanSecond) {
            if (source == null || source.Length == 0)
                return 0;

            T currentValue = source[0];
            long minIndex = 0;

            for (int index = 0; index < source.Length; index++)
                if (firstLessThanSecond(source[index], currentValue)) {
                    currentValue = source[index];
                    minIndex = index;
                }

            return minIndex;
        }

        /// <summary>
        /// Populates the source array with the result of the specified setter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="setter">A setter function accepting an index, returning a value.</param>
        /// <returns>Returns the source array with each element populated with the result of the specified setter function</returns>
        static public T[] Populate<T>(this T[] source, Func<long, T> setter) {
            if (source != null)
                for (long index = 0; index < source.Length; ++index)
                    source[index] = setter(index);

            return source;
        }

        /// <summary>
        /// Populates the source array with instances of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <returns>Returns the source array with each element populated with an instance of the specified type T</returns>
        static public T[] Populate<T>(this T[] source) where T : class, new() {
            if (source != null)
                for (long index = 0; index < source.Length; ++index)
                    source[index] = new T();

            return source;
        }

        /// <summary>
        /// Populates the source array with the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns the source array with each element populated with the specified value</returns>
        static public T[] Populate<T>(this T[] source, T value) {
            if (source != null)
                for (long index = 0; index < source.Length; ++index)
                    source[index] = value;

            return source;
        }

        /// <summary>
        /// Returns a copy of the source array with the specified element removed from it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="index">The zero-based index of the element to renmove.</param>
        /// <returns></returns>
        static public T[] Remove<T>(this T[] source, int index) {
            if (source != null) {
                List<T> values = source.ToList(); 

                values.RemoveAt(index);
                source = values.ToArray();
            }

            return source;
        }

        /// <summary>
        /// Returns a copy of the source array with the specified range of elements removed from it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <returns></returns>
        static public T[] Remove<T>(this T[] source, int index, int count = 1) {
            if (source != null) {
                List<T> values = source.ToList();

                values.RemoveRange(index, count);
                source = values.ToArray();
            }

            return source;
        }

        /// <summary>
        /// Determines whether the array is null or empty.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>
        ///   <c>true</c> if the specified array is null or empty; otherwise, <c>false</c>.
        /// </returns>
        static public bool IsNullOrEmpty(this Array array) {
            return array == null || array.Length == 0;
        }
    }
}
