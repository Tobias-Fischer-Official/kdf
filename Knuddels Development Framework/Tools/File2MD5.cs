using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.Tools
{
    public class File2MD5
    {
        public static string GetMD5(string filepath)
        {
            System.IO.FileStream fileCheck = System.IO.File.OpenRead(filepath);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] md5Hash = md5.ComputeHash(fileCheck);
            fileCheck.Close();
            return BitConverter.ToString(md5Hash).Replace("-", "").ToLower();
        }
    }
}
