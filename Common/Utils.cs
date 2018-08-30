/*
Sniperkit-Bot
- Status: analyzed
*/

﻿using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Bookmarks.Common
{
    public static class Utils
    {
        static Regex regex = new Regex(@"[-!_\s]+");

        public static string SanitizeStr(string t)
        {
            return regex.Replace(t, "").ToLowerInvariant();
        }

        public static string ComputeHash(string str2hash, HashAlgorithm hashAlgo)
        {
            if(string.IsNullOrEmpty(str2hash))
                return string.Empty;

            return string.Join(string.Empty
                             , hashAlgo.ComputeHash(Encoding.ASCII.GetBytes(str2hash))
                                                                  .Select(b => b.ToString("X2")));
        }
    }
}
