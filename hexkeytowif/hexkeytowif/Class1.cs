using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hexkeytowif
{
    class Class1
    {
        static void Main(String[] args)
        {
            ConsoleKeyInfo cki1;
            StringBuilder forFile = new StringBuilder();
            Console.WriteLine("This program will convert hex format private keys to WIF format keys. \r\n");
            Console.WriteLine("Do you want converted keys written to a file on the desktop? (y/n)");
            cki1 = Console.ReadKey();
            Console.Clear();
            string base58Encoded2 = ProgramMain.MainHexToWif();
            forFile.AppendLine(base58Encoded2);
            Console.WriteLine("");
            Console.WriteLine("Final WIF Format: " + base58Encoded2);
            Console.WriteLine("");
            Console.WriteLine("Press ENTER to continue or ESC to close.");

            ConsoleKeyInfo cki2;
            cki2 = Console.ReadKey();
            if (cki2.Key == ConsoleKey.Enter)
            {
                Console.Clear();
                base58Encoded2 = ProgramMain.MainHexToWif();
                Console.WriteLine("Gets here...");
                Console.ReadKey();
            }
            else if (cki2.Key == ConsoleKey.Escape && cki1.Key == ConsoleKey.Y)
            {

                string filePath = Path.Combine(
                   Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                  "BITCOIN_WIF_KEYS.txt");
                System.IO.File.WriteAllText(filePath, forFile.ToString());
                return;
            }
            else if (cki2.Key == ConsoleKey.Escape && cki1.Key == ConsoleKey.N)
            {
                return;
            }
            else
            {
                Console.Clear();
                base58Encoded2 = ProgramMain.MainHexToWif();
            }

        }
    }
}
