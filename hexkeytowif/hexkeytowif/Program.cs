using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


namespace hexkeytowif
{
    class ProgramMain
    {
        static void Main(string[] args)
        {
            int initialByte = 80;
            Console.WriteLine("Insert the private key as hex:");
            string hexKeyOG = Console.ReadLine();

            Console.WriteLine("Performing conversion...");

            string hexKeyStepOne = initialByte + hexKeyOG;
            
            Console.WriteLine("Key with 0x80 byte added: " + hexKeyStepOne);
            Console.ReadKey();

            string hexKeyStepTwo = (sha256_hash(hexKeyStepOne));

            Console.WriteLine("First SHA hashed key: " + hexKeyStepTwo);
            Console.ReadKey();

            string hexKeyStepThree = (sha256_hash(hexKeyStepTwo));

            Console.WriteLine("Second SHA hashed key: " + hexKeyStepThree);
            Console.ReadKey();

            string hexKeySubstring = hexKeyStepThree.Substring(0, 8);

            Console.WriteLine("First 4 bytes of 4 from second hashing: " + hexKeySubstring);
            Console.ReadKey();

            string hexHash1PlusSubstring = hexKeyStepOne + hexKeySubstring;

            Console.WriteLine("First SHA hash with 4 bytes of 4 appended: " + hexHash1PlusSubstring);
            Console.ReadKey();

            byte[] base58EncodedPrelim = StringToByteArray(hexHash1PlusSubstring);
            string base58Encoded = Base58Encoding.Encode(base58EncodedPrelim);

            Console.WriteLine("Final WIF Format: " + base58Encoded);
            Console.ReadKey();

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
