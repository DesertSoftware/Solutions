//
//  Copyright 2013, Desert Software Solutions Inc.
//    SecretConsole.cs:
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
using System.Security;
using System.Text;

namespace DesertSoftware.Solutions
{
    /// <summary>
    /// Provides functionality to read masked lines of characters from the standard input stream
    /// </summary>
    public class SecretConsole
    {
        /// <summary>
        /// Reads the next masked line of characters from the standard input stream
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns></returns>
        static public string ReadLine(char mask = '*') {
            var s = new StringBuilder();

            do {
                ConsoleKeyInfo keyPress = Console.ReadKey(true);

                // enter key pressed; end of line
                if (keyPress.Key == ConsoleKey.Enter) {
                    Console.WriteLine();
                    break;
                }

                // if we are not already at the beginning of the line backspace and remove previous mask character
                if (keyPress.Key == ConsoleKey.Backspace) {
                    if (s.Length > 0) {
                        s.Length--;
                        Console.Write("\b \b");
                    }

                    continue;
                }

                // if not a control character, collect the pressed character
                if (!char.IsControl(keyPress.KeyChar)) {
                    s.Append(keyPress.KeyChar);
                    Console.Write(mask);
                }
            } while (true);

            // return the string of characters read
            return s.ToString();
        }

        /// <summary>
        /// Reads the next masked line of characters from the standard input stream into a SecureString 
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns></returns>
        static public SecureString ReadSecureLine(char mask = '*') {
            var s = new SecureString();

            do {
                ConsoleKeyInfo keyPress = Console.ReadKey(true);

                // enter key pressed; end of line
                if (keyPress.Key == ConsoleKey.Enter) {
                    Console.WriteLine();
                    break;
                }

                // if we are not already at the beginning of the line backspace and remove previous mask character
                if (keyPress.Key == ConsoleKey.Backspace) {
                    if (s.Length > 0) {
                        s.RemoveAt(s.Length - 1);
                        Console.Write("\b \b");
                    }

                    continue;
                }

                // if not a control character, collect the pressed character
                if (!char.IsControl(keyPress.KeyChar)) {
                    s.AppendChar(keyPress.KeyChar);
                    Console.Write(mask);
                }

            } while (true);

            // return the secure string
            return s;
        }
    }
}
