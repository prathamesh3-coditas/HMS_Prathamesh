using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApplication1.Encryption
{
    public static class Encryption
    {
        public static string EncodePassword(string oldPassword)
        {
            var hashObj = MD5.Create();
            var byteArrayOfHash = hashObj.ComputeHash(Encoding.UTF8.GetBytes(oldPassword));

            var EncodedPassword = System.Text.Encoding.UTF8.GetString(byteArrayOfHash).Replace("\n","");
            EncodedPassword = EncodedPassword.Replace("\t", "");
            return EncodedPassword;
            
        }
    }
}