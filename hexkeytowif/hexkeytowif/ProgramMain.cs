using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace HexKeyToWifConverter
{
    /// <summary>
    /// A utility to convert Bitcoin private keys from hexadecimal format to Wallet Import Format (WIF).
    /// Supports single key conversion and bulk processing from a text file.
    /// </summary>
    public static class Program
    {
        private const string Base58Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private const int HexKeyLength = 64;
        private const byte MainNetPrefix = 0x80; // 128 in decimal

        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        public static void Main()
        {
            Console.Title = "Bitcoin Hex to WIF Converter";
            var processedKeys = new List<string>();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- Bitcoin Hex to WIF Converter ---");
                Console.WriteLine("1. Convert a single key");
                Console.WriteLine("2. Convert multiple keys from .txt file. Includes option to save output as .txt file.");
                Console.WriteLine("3. Exit");
                Console.Write("\nPlease select an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        ProcessSingleKey(processedKeys);
                        break;
                    case "2":
                        ProcessBulkFile(processedKeys);
                        break;
                    case "3":
                        return; // Exit the application
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Handles the conversion of a single key provided by the user.
        /// </summary>
        private static void ProcessSingleKey(List<string> processedKeys)
        {
            Console.Clear();
            Console.WriteLine("--- Single Key Conversion ---");
            Console.Write("Please enter the 64-character hex private key: ");
            string hexKey = Console.ReadLine()?.Trim();

            string wifKey = ConvertHexToWif(hexKey);

            Console.WriteLine($"\nHex Input: {hexKey}");
            Console.WriteLine($"WIF Output: {wifKey}");

            if (!wifKey.StartsWith("INVALID"))
            {
                processedKeys.Add($"Hex: {hexKey}, WIF: {wifKey}");
            }
        }

        /// <summary>
        /// Handles the bulk conversion of keys from a user-specified text file.
        /// </summary>
        private static void ProcessBulkFile(List<string> processedKeys)
        {
            Console.Clear();
            Console.WriteLine("--- Bulk File Conversion ---");
            Console.Write("Please enter the full path to your .txt file. Keys should be one per line: ");
            string filePath = Console.ReadLine()?.Trim('"'); // Allow dropping file path

            if (!File.Exists(filePath))
            {
                Console.WriteLine("\nError: File not found.");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                var validKeys = lines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => line.Trim()).ToList();

                if (!validKeys.Any())
                {
                    Console.WriteLine("\nFile does not contain any keys to process.");
                    return;
                }

                Console.WriteLine($"\nFound {validKeys.Count} keys to process. Starting conversion...");
                var wifKeysForFile = new List<string>();
                foreach (string hexKey in validKeys)
                {
                    string wifKey = ConvertHexToWif(hexKey);
                    string consoleOutput = $"Hex: {hexKey}, WIF: {wifKey}";
                    Console.WriteLine(consoleOutput); // Show full details on screen

                    // Add only the valid WIF key to the list that will be saved to the file
                    if (!wifKey.StartsWith("INVALID") && !wifKey.StartsWith("CONVERSION_ERROR"))
                    {
                        wifKeysForFile.Add(wifKey);
                    }
                }

                // Only offer to save if there are valid keys to save
                if (wifKeysForFile.Any())
                {
                    OfferToSaveResults(wifKeysForFile);
                }
                else
                {
                    Console.WriteLine("\nNo valid WIF keys were generated to save.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred while reading the file: {ex.Message}");
            }
        }

        /// <summary>
        /// Asks the user if they want to save the conversion results to a file.
        /// </summary>
        private static void OfferToSaveResults(List<string> wifKeys)
        {
            Console.Write("\nDo you want to save the converted WIF keys to a file on your desktop? (y/n): ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                try
                {
                    string fileName = $"WIF_Keys_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string fullPath = Path.Combine(desktopPath, fileName);

                    File.WriteAllLines(fullPath, wifKeys);
                    Console.WriteLine($"\n\nResults successfully saved to: {fullPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n\nError saving file: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Core logic to convert a single hexadecimal private key to WIF.
        /// </summary>
        /// <param name="hexKey">The 64-character private key in hex format.</param>
        /// <returns>The WIF formatted key or an error message.</returns>
        public static string ConvertHexToWif(string hexKey)
        {
            if (string.IsNullOrWhiteSpace(hexKey) || hexKey.Length != HexKeyLength)
            {
                return $"INVALID_LENGTH (Expected {HexKeyLength} chars)";
            }

            try
            {
                // 1. Add 0x80 prefix for MainNet
                string extendedHexKey = MainNetPrefix.ToString("X2") + hexKey;

                // 2. Perform SHA-256 hash on the extended key
                byte[] extendedKeyBytes = HexStringToBytes(extendedHexKey);
                byte[] firstHash = SHA256.Create().ComputeHash(extendedKeyBytes);

                // 3. Perform SHA-256 hash on the result of the previous hash
                byte[] secondHash = SHA256.Create().ComputeHash(firstHash);

                // 4. Take the first 4 bytes of the second hash as a checksum
                string checksum = BitConverter.ToString(secondHash, 0, 4).Replace("-", "");

                // 5. Append the checksum to the extended key from step 1
                byte[] finalBytes = HexStringToBytes(extendedHexKey + checksum);

                // 6. Encode the result in Base58
                return Base58Encode(finalBytes);
            }
            catch (FormatException)
            {
                return "INVALID_FORMAT (Input was not a valid hex string)";
            }
            catch (Exception ex)
            {
                return $"CONVERSION_ERROR: {ex.Message}";
            }
        }

        /// <summary>
        /// Converts a string of hexadecimal characters to a byte array.
        /// </summary>
        private static byte[] HexStringToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Encodes a byte array into a Base58 string.
        /// </summary>
        private static string Base58Encode(byte[] data)
        {
            // Decode byte[] to BigInteger
            var intData = new BigInteger(data.Reverse().Concat(new byte[] { 0 }).ToArray());

            // Encode BigInteger to Base58 string
            var result = new StringBuilder();
            while (intData > 0)
            {
                intData = BigInteger.DivRem(intData, 58, out BigInteger remainder);
                result.Insert(0, Base58Alphabet[(int)remainder]);
            }

            // Append `1` for each leading 0 byte
            for (int i = 0; i < data.Length && data[i] == 0; i++)
            {
                result.Insert(0, '1');
            }

            return result.ToString();
        }
    }
}