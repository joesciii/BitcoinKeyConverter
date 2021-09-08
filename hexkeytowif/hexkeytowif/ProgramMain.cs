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
            Console.WriteLine("This program will convert hex format private keys to WIF format keys. \r\n");
            Console.WriteLine("Do you want converted keys written to a file on the desktop? (y/n)");            
            cki1 = Console.ReadKey();
            Console.Clear();
            //runs forever for bulk changing of keys
            while (true)
            {
                int initialByte = 80;
                initialInput:  Console.WriteLine("Insert the private key as hex:");

                string hexKeyOG = Console.ReadLine();

                if(hexKeyOG.Length != 64)
                {
                    Console.Clear();
                    Console.WriteLine("Your input was incorrect. \r\nThis program takes 64 character hex format bitcoin keys only.\r\n");
                    goto initialInput;
                }

                string hexKeyStepOne = initialByte + hexKeyOG;
                //Console.WriteLine("Key with 0x80 byte added: " + hexKeyStepOne);
                string hexKeyStepTwo = (SHA256Hash(hexKeyStepOne));
                //Console.WriteLine("First SHA hashed key: " + hexKeyStepTwo);
                string hexKeyStepThree = (SHA256Hash(hexKeyStepTwo));
                //Console.WriteLine("Second SHA hashed key: " + hexKeyStepThree);
                string hexKeySubstring = hexKeyStepThree.Substring(0, 8);
                //Console.WriteLine("First 4 bytes of 4 from second hashing: " + hexKeySubstring);
                string hexHash1PlusSubstring = hexKeyStepOne + hexKeySubstring;
                //Console.WriteLine("First SHA hash with 4 bytes of 4 appended: " + hexHash1PlusSubstring);
                byte[] base58EncodedPrelim = StringToBytes(hexHash1PlusSubstring);
                string base58Encoded = Base58Encode(base58EncodedPrelim);


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

        public static byte[] StringToBytes(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static String SHA256Hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Byte[] result = hash.ComputeHash(StringToBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        
        public static string Base58Encode(byte[] data)
        {
            Contract.Requires(data != null);
            Contract.Ensures(Contract.Result<string>() != null);

            string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

            // Decode byte[] to BigInteger
            BigInteger intData = 0;
            for (int i = 0; i < data.Length; i++)
            {
                intData = intData * 256 + data[i];
            }

            // Encode BigInteger to Base58 string
            string result = "";
            while (intData > 0)
            {
                int remainder = (int)(intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (int i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }
            return result;
        }

    }
}
