using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.Networks.Protocol
{
    internal class Password
    {
        internal static string Encrypt(string pStr, string pKey)
        {
            if (pStr == null || pStr.Length <= 0 || pKey == null || pKey.Length <= 0)
                return string.Empty;
            int strLength = pStr.Length;
            int keyLength = pKey.Length;
            int k = strLength ^ keyLength << 4;
            int bufferLength = keyLength > strLength ? keyLength : strLength;
            StringBuilder buffer = new StringBuilder(bufferLength);

            for (int i = 0; i < bufferLength; i++)
            {
                buffer.Append((char)(pStr[i % strLength] ^ pKey[i % keyLength] ^ k));
            }

            return buffer.ToString();
        }

        internal static int Hash(string pStr)
        {
            int i = 0;
            int j = 0;
            int strLength = pStr.Length;

            if (strLength < 19)
            {
                for (int n = strLength - 1; n >= 0; n--)
                {
                    i = i * 3 + pStr[n];
                    j = j * 5 + pStr[strLength - n - 1];
                }
            }
            else
            {
                int m = strLength / 19;
                int n = strLength - 1;

                while (n >= 0)
                {
                    i = i * 5 + pStr[n];
                    j = j * 3 + pStr[strLength - n - 1];
                    n -= m;
                }
            }

            int k = i ^ j;
            return k & 0xFFFFFF ^ k >> 24;
        }
    }
}
