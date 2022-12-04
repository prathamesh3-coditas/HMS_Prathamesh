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
            var hashObj = SHA256.Create();
            var byteArrayOfHash = hashObj.ComputeHash(Encoding.UTF8.GetBytes(oldPassword));

            //Convert.toHexString()
            var EncodedPassword = BitConverter.ToString(byteArrayOfHash).Replace("-", "");
            return EncodedPassword;
            
        }
    }
}