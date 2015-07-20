using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Guitar32.Cryptography
{
    /// <summary>
    /// Utility class for SHA256 Hash implementation
    /// </summary>
    public class SHA256Hash
    {

        /// <summary>
        /// Compute the SHA256 hash from an input string
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The computer SHA256 hash string</returns>
        static public string Compute(string input)
        {
            SHA256 sha = new SHA256Managed();
            byte[] buffer = sha.ComputeHash(Encoding.ASCII.GetBytes(input));

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                stringBuilder.Append(buffer[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Verify if a raw input string matches a SHA256 hash
        /// </summary>
        /// <param name="input">The raw input string</param>
        /// <param name="sha256hash">The SHA256 hash to be matched</param>
        /// <returns>If input matches the SHA256 hash or not</returns>
        static public bool Verify(string input, string sha256hash)
        {
            string hash = sha256hash;
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hash, sha256hash) == 0;
        }

    }
}
