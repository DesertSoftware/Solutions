//
//  Copyright 2015, Desert Software Solutions Inc.
//    ArrayFormatter.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Formatters/ArrayFormatter.cs
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

namespace DesertSoftware.Solutions.Formatters
{
    static public class ArrayFormatter
    {
        /// <summary>
        /// Dumps the array of words to a string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="index">The starting index (default 0).</param>
        /// <param name="length">The number of values to dump (default all values in array).</param>
        /// <returns></returns>
        static public string DumpToString(this short[] values, int index = 0, int length = -1) {
            length = length < 1 ? values.Length : length;

            using (StringWriter writer = new StringWriter()) {
                while (length-- > 0) {
                    byte[] bytes = BitConverter.GetBytes(values[index++]);

                    writer.Write("{0:x2} {1:x2} ", bytes[1], bytes[0]);
                }

                return writer.ToString();
            }
        }

        static public void DumpToHex() {

        }
    }


    // https://codereview.stackexchange.com/questions/145506/printing-hex-dump-of-a-byte-array
    // Usage:
    // Console.OutputEncoding = Encoding.GetEncoding(1252);
    // byte[] example = Enumerable.Range(0, 256).Select(x=>(byte)x).ToArray();
    // Console.WriteLine(Hex.Dump(example));

    // NOTES:
    // Thing I like less is that you're forcing caller to deal with a static method. I appreciate that kind of helpers but they should be an alternative 
    // (maybe with default settings) instead of the only way to go. Why?

    // If you need more parameters (or to reorder them) then you need to change all calling points.
    // If you add complex parameters (such as a formatter) then calling point will quickly become a mess.
    // They're viral. If you separate UI from underlying logic and you need to delegate some decisions to different places then you can't pass around an Hex class 
    // (where each component/control may fill its own properties) because you have a single method call.
    // If a static method becomes complex enough then it will also be harder to test.
    // I'd also change name from Hex to something that describe what your class is doing. HexStringFormatter? HexStringConverter? I'd give two alternatives:

    // var formatter = new HexStringFormatter();
    // Console.WriteLine(formatter.ConvertToString(example));
    // Note that in this simple case it may even be (an helper method here just save few characters, what's for?):

    // Console.WriteLine(new HexStringFormatter().ConvertToString(example));
    // A more complex case may be like this:

    // var formatter = new HexStringFormatter();
    // formatter.BytesPerLine = 32;
    // formatter.Content = HexStringFormatterOutput.Ascii
    // HexStringFormatterOutput.Header;
    // These settings may come from UI, now you can return formatter object to another method for example in charge to read data without knowing where settings come from...

    // Note that I also introduced an enum instead of multiple boolean properties but it's not mandatory and not even always suggested, it depends case by case 
    // (I like the enum because I can save it in one shot without any effort in my configuration files).

    // Also you may want to make Hex sealed, it's not intended (so far) to be extended.
    
    // You may accept IEnumerable<byte> instead of byte[]. Performance impact in WriteBody() with foreach is negligible, and you do not need ToArray() in your example. 
    // Not such big gain but it will let you read huge files that does not fit in memory (because you can read them block by block without need to convert to byte[] 
    // all the content).

    // In this regard you may want to change the output of your class. Now you need to build a huge output string, if content will be written to disk then it's a waste 
    // of resources. Write output to a TextWriter. Caller will have responsibility to give you a StringWriter if he wants to output to a string. In this case it may be nice 
    // to introduce a static helper method for the most common use case. More complex use case may be like this:

    // using (var output = new StringWriter())
    // {
    //     var formatter = new HexStringFormatter();
    //     formatter.Output = output;
    //     formatter.BytesPerLine = 32;

    //     formatter.ConvertToString(example);
    //     Console.WriteLine(output.ToString());
    // }

    // HOWEVER note that HexStringFormatter accepts a TextWriter and Console.Out is a TextWriter! Code will then be simplified to:

    // var formatter = new HexStringFormatter();
    // formatter.Output = Console.Out;
    // formatter.BytesPerLine = 32;

    // formatter.ConvertToString(example);
    // What if you need to write it to a text file?

    // using (var stream = new StreamWriter("path_to_file"))
    // {
    //     var formatter = new HexStringFormatter();
    //     formatter.Output = stream;
    //     formatter.BytesPerLine = 32;

    //     formatter.ConvertToString(example);
    // }

    // Of course you may want to introduce few helper methods for this (now they can make sense but keep them as simple as possible), something like this:

    // public static string DumpToString(IEnumerable<byte> data)
    // {
    //     using (var output = new StringWriter())
    //     {
    //         var formatter = new HexStringFormatter();
    //         formatter.Output = output;

    //         formatter.ConvertToString(data);
    //         return output.ToString();
    //     }
    // }

    // public static void DumpToFile(string path, IEnumerable<byte> data)
    // {
    //     using (var output = new StreamWriter(path))
    //     {
    //         var formatter = new HexStringFormatter();
    //         formatter.Output = output;

    //         formatter.ConvertToString(data);
    //     }
    // }

    // public static void DumpToConsole(IEnumerable<byte> data)
    // {
    //     var formatter = new HexStringFormatter();
    //     formatter.Output = Console.Out;

    //     formatter.ConvertToString(data);
    // }

    // Now your first example will be again like this:

    // HexStringFormatter.DumpToConsole(
    //     Enumerable.Range(0, 256).Select(x => (byte)x));

    // Few other minor things. Your Translate() function should be simplified and made static). Also do not need to get a new encoding for each call, use Encoding.ASCII. 
    // b < 32 is clear for most of us but I'd make it explicit. For now let's keep GetString() but see later...

    // private static string Translate(byte b)
    // {
    //     if (IsPrintableCharacter(b))
    //         return Encoding.ASCII.GetString(new[] { b });

    //     return "."
    // }

    // private static bool IsPrintableCharacter(byte b) => b > 32;
    
    // Now let's think...you're working with ASCII and byte is in range [0...255] then you may directly cast it to char (after leave to representation issues you mentioned
    // in your Console codepage settings), you avoid to create so many strings and you do not even need any encoding:

    // private static char Translate(byte b)
    //     => IsPrintableCharacter(b) ? (char)b : '.';

    // Code like i & 0xFF in WriteHeader() is little bit misleading, IMO. If _bytesPerLine contains an invalid value then it should throw ArgumentOutOfRangeException when 
    // you set it, it shouldn't be silently ignored. Your code will then be simplified everywhere.

    // About your main loop, you do not need to save _length field because you already have the input buffer. I'd also try to give names to all those ifs 
    // (=introduce bool locals or separate functions) anyway after refactoring to use IEnumerable<byte> it should look much simpler.

    public class HexFormatter
    {
        private readonly byte[] _bytes;
        private readonly int _bytesPerLine;
        private readonly bool _showHeader;
        private readonly bool _showOffset;
        private readonly bool _showAscii;

        private readonly int _length;

        private int _index;
        private readonly StringBuilder _sb = new StringBuilder();

        private HexFormatter(byte[] bytes, int bytesPerLine, bool showHeader, bool showOffset, bool showAscii) {
            _bytes = bytes;
            _bytesPerLine = bytesPerLine;
            _showHeader = showHeader;
            _showOffset = showOffset;
            _showAscii = showAscii;
            _length = bytes.Length;
        }

        public static string Dump(byte[] bytes, int bytesPerLine = 16, bool showHeader = true, bool showOffset = true, bool showAscii = true) {
            if (bytes == null) {
                return "<null>";
            }

            return (new HexFormatter(bytes, bytesPerLine, showHeader, showOffset, showAscii)).Dump();
        }

        private string Dump() {
            if (_showHeader) {
                WriteHeader();
            }

            WriteBody();
            return _sb.ToString();
        }

        private void WriteHeader() {
            if (_showOffset) {
                _sb.Append("Offset(h)  ");
            }

            for (int i = 0; i < _bytesPerLine; i++) {
                //            _sb.Append($"{i & 0xFF:X2}");
                _sb.AppendFormat("{0:X2}", i & 0xFF);
                if (i + 1 < _bytesPerLine) {
                    _sb.Append(" ");
                }
            }

            _sb.AppendLine();
        }

        private void WriteBody() {
            while (_index < _length) {
                if (_index % _bytesPerLine == 0) {
                    if (_index > 0) {
                        if (_showAscii) {
                            WriteAscii();
                        }
                        _sb.AppendLine();
                    }

                    if (_showOffset) {
                        WriteOffset();
                    }
                }

                WriteByte();
                if (_index % _bytesPerLine != 0 && _index < _length) {
                    _sb.Append(" ");
                }
            }

            if (_showAscii) {
                WriteAscii();
            }
        }

        private void WriteOffset() {
            //        _sb.Append($"{_index:X8}   ");
            _sb.AppendFormat("{0:X8}   ", _index);
        }

        private void WriteByte() {
            //        _sb.Append(@"{_bytes[_index]:X2}");
            _sb.AppendFormat("{0:X2}", _bytes[_index]);
            _index++;
        }

        private void WriteAscii() {
            int backtrack = ((_index - 1) / _bytesPerLine) * _bytesPerLine;
            int length = _index - backtrack;

            // This is to fill up last string of the dump if it's shorter than _bytesPerLine
            _sb.Append(new string(' ', (_bytesPerLine - length) * 3));

            _sb.Append("   ");
            for (int i = 0; i < length; i++) {
                _sb.Append(Translate(_bytes[backtrack + i]));
            }
        }

        private string Translate(byte b) {
            return b < 32 ? "." : Encoding.GetEncoding(1252).GetString(new[] { b });
        }
    }
}
