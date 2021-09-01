using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;

namespace hexkeytowif
{
    class ProgramMain
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo cki1;
            StringBuilder forFile = new StringBuilder();
            Console.WriteLine("ENSURE YOU ARE RUNNING AS ADMINISTRATOR IF YOU WANT FILE WRITING PRIVELIGES! \r\n");
            Console.WriteLine("Do you want converted keys written to a file on the desktop? (y/n)");            
            cki1 = Console.ReadKey();
            Console.Clear();
            //runs forever for bulk changing of keys
            while (true)
            {
                int initialByte = 80;
                Console.WriteLine("Insert the private key as hex:");

                string hexKeyOG = Console.ReadLine();

                string hexKeyStepOne = initialByte + hexKeyOG;
                //Console.WriteLine("Key with 0x80 byte added: " + hexKeyStepOne);
                string hexKeyStepTwo = (sha256_hash(hexKeyStepOne));
                //Console.WriteLine("First SHA hashed key: " + hexKeyStepTwo);
                string hexKeyStepThree = (sha256_hash(hexKeyStepTwo));
                //Console.WriteLine("Second SHA hashed key: " + hexKeyStepThree);
                string hexKeySubstring = hexKeyStepThree.Substring(0, 8);
                //Console.WriteLine("First 4 bytes of 4 from second hashing: " + hexKeySubstring);
                string hexHash1PlusSubstring = hexKeyStepOne + hexKeySubstring;
                //Console.WriteLine("First SHA hash with 4 bytes of 4 appended: " + hexHash1PlusSubstring);
                byte[] base58EncodedPrelim = StringToByteArray(hexHash1PlusSubstring);
                string base58Encoded = Base58Encoding.Encode(base58EncodedPrelim);


                forFile.AppendLine(base58Encoded);
                Console.WriteLine("");
                Console.WriteLine("Final WIF Format: " + base58Encoded);
                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue or ESC to close.");

                ConsoleKeyInfo cki2;
                cki2 = Console.ReadKey();
                if(cki2.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                }
                else if(cki2.Key == ConsoleKey.Escape && cki1.Key == ConsoleKey.Y)
                {

                    string filePath = Path.Combine(
                       Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                      "BITCOIN_WIF_KEYS.txt");
                    System.IO.File.WriteAllText(filePath, forFile.ToString());
                    break;
                }
                else if(cki2.Key == ConsoleKey.Escape && cki1.Key == ConsoleKey.N)
                {
                    break;
                }
                else
                {
                    Console.Clear();
                }
            }
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Byte[] result = hash.ComputeHash(StringToByteArray(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

    }
}
