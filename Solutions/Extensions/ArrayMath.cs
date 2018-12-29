using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesertSoftware.Solutions.Extensions
{
    static public class ArrayMath
    {
        // Array Math functionality

        /// <summary>
        /// Adds the specified value to each element of the source array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        static public T[] Add<T>(this T[] source, T value) {
            if (source == null)
                return source;

            for (long index = 0; index < source.Length; index++)
                source[index] = (dynamic)source[index] + (dynamic)value;

            return source;
        }

        /// <summary>
        /// Adds the specified element value in values to its corresponding source array element. Source and values must be equally sized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="values">The values array.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">the number of elements in values does not match the number within source</exception>
        static public T[] Add<T>(this T[] source, T[] values) {
            if (source == null || values == null)
                return source;

            if (source.Length != values.Length) throw new ArgumentException("the number of elements in values does not match the number within source");

            for (long index = 0; index < source.Length; index++)
                source[index] = (dynamic)source[index] + (dynamic)values[index];

            return source;
        }

        /// <summary>
        /// Divides each element of the source array by the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        static public T[] Divide<T>(this T[] source, T value) {
            if (source == null)
                return source;

            for (long index = 0; index < source.Length; index++)
                source[index] = (dynamic)source[index] / (dynamic)value;

            return source;
        }

        /// <summary>
        /// Divides each element of the source array by the corresponding value in the values array. Source and values must be equally sized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="values">The values array.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">the number of elements in values does not match the number within source</exception>
        static public T[] Divide<T>(this T[] source, T[] values) {
            if (source == null || values == null)
                return source;

            if (source.Length != values.Length) throw new ArgumentException("the number of elements in values does not match the number within source");

            for (long index = 0; index < source.Length; index++)
                source[index] = (dynamic)source[index] / (dynamic)values[index];

            return source;
        }

        /// <summary>
        /// Multiplies each element of the source array by the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        static public T[] Multiply<T>(this T[] source, T value) {
            if (source == null)
                return source;

            for (long index = 0; index < source.Length; index++)
                source[index] = (dynamic)source[index] * (dynamic)value;

            return source;
        }

        /// <summary>
        /// Multiplies each element of the source array by the corresponding value in the values array. Source and values must be equally sized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="values">The values array.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">the number of elements in values does not match the number within source</exception>
        static public T[] Multiply<T>(this T[] source, T[] values) {
            if (source == null || values == null)
                return source;

            if (source.Length != values.Length) throw new ArgumentException("the number of elements in values does not match the number within source");

            for (long index = 0; index < source.Length; index++)
                source[index] = (dynamic)source[index] * (dynamic)values[index];

            return source;
        }

        /// <summary>
        /// Subtracts the specified value from each element in the source array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        static public T[] Subtract<T>(this T[] source, T value) {
            if (source == null)
                return source;

            for (long index = 0; index < source.Length; index++)
                source[index] = (dynamic)source[index] - (dynamic)value;

            return source;
        }

        /// <summary>
        /// Subtracts the specified element value in values to its corresponding source array element. Source and values must be equally sized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="values">The values array.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">the number of elements in values does not match the number within source</exception>
        static public T[] Subtract<T>(this T[] source, T[] values) {
            if (source == null || values == null)
                return source;

            if (source.Length != values.Length) throw new ArgumentException("the number of elements in values does not match the number within source");

            for (long index = 0; index < source.Length; index++)
                source[index] = (dynamic)source[index] - (dynamic)values[index];

            return source;
        }

    }
}
