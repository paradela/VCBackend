﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CryptSharp;

namespace VCBackend.Utility.Security
{
    public class Pbkdf2
    {
        public static String DeriveKey(String Password) {

            byte[] bytesPwd = System.Text.Encoding.UTF8.GetBytes(Password);
            var algo = new System.Security.Cryptography.HMACSHA1(bytesPwd);

            var hashSize = 32;
            byte[] saltBytes = new byte[64];

            byte[] hashedPwd = CryptSharp.Utility.Pbkdf2.ComputeDerivedKey(algo, saltBytes, 1000, hashSize);

            String strPwd = System.Convert.ToBase64String(hashedPwd);

            return strPwd;
        }
    }
}