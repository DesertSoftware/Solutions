//
//  Copyright 2013, Desert Software Solutions Inc.
//    DateExtensions.cs: 
//      https://github.com/DesertSoftware/Solutions/blob/master/Solutions/Extensions/NumberExtensions.cs
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
        static public int[] Sequence(this int min, int max, int step = 1) {
            var values = new List<int>();

            for (var value = min; value <= max; value += step)
                values.Add(value);

            return values.ToArray();
        }
    }
}
