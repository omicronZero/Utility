using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Utility
{
    internal static class NameHashHelper
    {
        private static readonly MD5 MD5Provider = MD5.Create();

        public static string MD5Name(string name)
        {
            byte[] cb = MD5Provider.ComputeHash(Encoding.Unicode.GetBytes(name));

            StringBuilder sb = new StringBuilder(cb.Length * 2);

            for (int i = 0; i < cb.Length; i++)
            {
                byte c = cb[i];

                sb.Append((char)('A' + (c & 0xf)));

                sb.Append((char)('A' + (c >> 4)));
            }

            return sb.ToString();
        }
    }
}
