using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace LetPortal.Core.Utils
{
    public class StringUtil
    {
        public static string EncodeBase64FromUTF8(string encodingString)
        {
            if (string.IsNullOrEmpty(encodingString))
            {
                return null;
            }
            var encodedBytes = Encoding.UTF8.GetBytes(encodingString);
            var encodedTxt = Convert.ToBase64String(encodedBytes);
            return encodedTxt;
        }

        public static string DecodeBase64ToUTF8(string encodedString)
        {
            if (string.IsNullOrEmpty(encodedString))
            {
                return null;
            }
            var decodedBytes = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(decodedBytes);
        }

        public static string CompressionString(string compressingString)
        {
            var bytes = Encoding.Unicode.GetBytes(compressingString);
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    gs.Write(bytes, 0, bytes.Length);
                }
                return BitConverter.ToString(mso.ToArray());
            }
        }

        public static string DecompressionString(string compressionString)
        {
            var data = GetBytes(compressionString);
            // Read the last 4 bytes to get the length
            var lengthBuffer = new byte[4];
            Array.Copy(data, data.Length - 4, lengthBuffer, 0, 4);
            var uncompressedSize = BitConverter.ToInt32(lengthBuffer, 0);

            var buffer = new byte[uncompressedSize];
            using (var ms = new MemoryStream(data))
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    gzip.Read(buffer, 0, uncompressedSize);
                }
            }
            return Encoding.Unicode.GetString(buffer);
        }

        public static byte[] GetBytes(string str)
        {
            var arr = str.Split('-');
            var array = new byte[arr.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                array[i] = Convert.ToByte(arr[i], 16);
            }

            return array;
        }

        public static string EncryptionString(string plain, string key, string iv)
        {
            byte[] encrypted;
            using (var aes = new AesManaged())
            {
                var encryptor = aes.CreateEncryptor(GetBytes(key), GetBytes(iv));
                // Create MemoryStream    
                using (var ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plain);
                        }

                        encrypted = ms.ToArray();
                    }
                }
            }

            return BitConverter.ToString(encrypted);
        }

        public static string DecryptionString(string encryption, string key, string iv)
        {
            string plain = null;
            using (var aes = new AesManaged())
            {
                var decryptor = aes.CreateEncryptor(GetBytes(key), GetBytes(iv));
                // Create MemoryStream    
                using var ms = new MemoryStream(GetBytes(encryption));
                // Create crypto stream    
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                // Read crypto stream    
                using var reader = new StreamReader(cs);
                plain = reader.ReadToEnd();
            }

            return plain;
        }

        public static string[] GetAllDoubleCurlyBraces(string str, bool keptCurlyBraces = false, IEnumerable<string> removeList = null)
        {
            var matches = Regex.Matches(str, @"{{(?!\$)(.*?)}}");
            if (keptCurlyBraces)
            {
                var results = matches.Cast<Match>().Select(a => a.Groups[1].Value).Distinct().Select(b => "{{" + b + "}}").ToArray();
                if (removeList != null)
                {
                    results = results.AsQueryable().Where(a => !removeList.Any(b => b.Equals(a))).ToArray();
                }
                return results;
            }
            else
            {
                var results = matches.Cast<Match>().Select(a => a.Groups[1].Value).Distinct().ToArray();
                if (removeList != null)
                {
                    results = results.AsQueryable().Where(a => !removeList.Any(b => b.Equals(a))).ToArray();
                }
                return results;
            }
        }

        public static string ReplaceDoubleCurlyBraces(string str, IEnumerable<Tuple<string, string, bool>> tuples)
        {
            if (tuples == null)
            {
                return str;
            }

            foreach (var tuple in tuples)
            {
                if (tuple.Item3)
                {
                    str = str.Replace("\"{{" + tuple.Item1 + "}}\"", tuple.Item2);
                }
                else
                {
                    str = str.Replace("{{" + tuple.Item1 + "}}", tuple.Item2);
                }
            }

            return str;
        }

        public static string[] GetByRegexMatchs(string regex, string str, bool keptCurlyBraces = false, IEnumerable<string> removeList = null)
        {
            var matches = Regex.Matches(str, regex);
            if (keptCurlyBraces)
            {
                var results = matches.Cast<Match>().Select(a => a.Groups[1].Value).Distinct().Select(b => "{{" + b + "}}").ToArray();
                if (removeList != null)
                {
                    results = results.AsQueryable().Where(a => !removeList.Any(b => b.Equals(a, StringComparison.Ordinal))).ToArray();
                }
                return results;
            }
            else
            {
                var results = matches.Cast<Match>().Select(a => a.Groups[1].Value).Distinct().ToArray();
                if (removeList != null)
                {
                    results = results.AsQueryable().Where(a => !removeList.Any(b => b.Equals(a, StringComparison.Ordinal))).ToArray();
                }
                return results;
            }
        }

        public static string GenerateUniqueName(int length = 10)
        {
            var suppliedVars = "abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var lengthOfName = length;
            var datasourceName = string.Empty;
            for (var i = 0; i < lengthOfName; i++)
            {
                var randomIndx = (new Random()).Next(0, 45);
                datasourceName += suppliedVars[randomIndx];
            }

            return datasourceName;
        }

        public static string GenerateUniqueNumber()
        {
            var currentTickString = DateTime.UtcNow.Ticks.ToString();
            for (var i = 0; i < 5; i++)
            {
                var randomIndx = (new Random()).Next(0, 9);
                currentTickString += randomIndx.ToString();
            }
            return currentTickString;
        }

        public static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }
    }
}
