using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace hexkeytowif
{
    class Program
    {
        static void Main(string[] args)
        {
            int initialByte = 0x80;
            Console.WriteLine("Insert the private key as hex:");
            string hexKeyOG = Console.ReadLine();
            int hexKeyOGInt = Convert.ToInt32(hexKeyOG);
            Console.WriteLine("Performing conversion...");

            string hexKeyStepOne = initialByte + hexKeyOG;
            
            Console.WriteLine("Key with 0x80 byte added: " + hexKeyStepOne);
            Console.ReadKey();

            int hexKeyStepOneInt = Convert.ToInt32(hexKeyStepOne);
            string hexKeyStepTwo = (sha256_hash(hexKeyStepOne));

            Console.WriteLine("First SHA hashed key: " + hexKeyStepTwo);
            Console.ReadKey();
            //string hexKeyStepTwo = //SHA HASH STEP ONE
            //string hexKeyStepThree = //SHA HASH STEP TWO
            //TAKE FIRST 4 BYTES FROM STEPTHREE
            //ADD THESE 4 BYTES TO THE END OF STEPONE
            //CONVERT NEW STEPONE STRING TO BASE58

        }
        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
