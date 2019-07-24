/* 
//  Desert Software Solutions, Inc 2019
//  CSV Reader example

// http://www.desertsoftware.com

// THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
// BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY, NON-INFRINGEMENT AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED.  

// IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, WHETHER OR NOT SUCH DAMAGES WERE FORESEEABLE AND EVEN IF THE AUTHOR IS ADVISED 
// OF THE POSSIBILITY OF SUCH DAMAGES. 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DesertSoftware.Solutions.IO;

namespace Solutions.Examples
{
    class CsvDump
    {
        static void Main(string[] args) {
            // open the csv file (CsvReader)
            // read each line
            // display each column

//            using (CsvReader reader = new CsvReader("./Artifacts/CsvWithCRLF.csv")) { 
            using (CsvReader reader = new CsvReader("./Artifacts/CsvWithQuotesInValues.csv")) {
                foreach (var line in reader.ReadAllLines(false)) {
                    int index = 0;

                    foreach (var column in reader.Columns)
                        Console.WriteLine("{0}: {1}", column, reader.ColumnValue(line, index++));

                    Console.Write("continue? ");
                    char ch = Console.ReadKey().KeyChar;
                    if (ch != 'y' && ch != 'Y')
                        break;
                }
            }
        }
    }
}
