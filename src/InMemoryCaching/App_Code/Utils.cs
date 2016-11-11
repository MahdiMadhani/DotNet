using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCaching.App_Code
{
    public class Utils
    {      /// <summary>
           /// Read a stream which is comming from IFromFile interface
           /// </summary>
           /// <param name="sr"></param>
           /// <returns></returns>
        internal static MemoryStream LoadToMemoryStream(Stream sr)
        {

            MemoryStream ms = new MemoryStream();
            do
            {
                byte[] buff = new byte[1024];
                int len = sr.Read(buff, 0, buff.Length);
                if (len <= 0)
                    break;
                ms.Write(buff, 0, len);
            } while (true);
            return ms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        internal static byte[] ConvertMemoryStreamToBytes(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            byte[] fileData = new byte[ms.Length];
            int readLen = 0;
            do
            {

                byte[] buff = new byte[1024];
                int len = ms.Read(buff, 0, buff.Length);
                if (len > 0)
                {
                    Buffer.BlockCopy(buff, 0, fileData, readLen, len);
                    readLen += len;
                }
                else
                    break;
            } while (true);
            return fileData;
        }
        /// <summary>
        /// Creates HTTP response compatible response based on given raw image content
        /// </summary>
        /// <param name="imageType">it can be gif, jpeg, bmp</param>
        /// <returns></returns>
        public static string GenerateHttpResponseImageOntheFly(byte[] imageData, string imageType)
        {
            //' Set the content type and return the image
            MemoryStream ms = new MemoryStream(imageData);
            var base64Data = Convert.ToBase64String(ms.ToArray());
            var imgSrcData = string.Format("data:image/{0};base64,{1}", imageType, base64Data);
            ms.Dispose();
            return imgSrcData;
        }

        public static string GenerateHashPassword(string password)
        {
            //var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            //var sha1data = sha1.ComputeHash(
            //    System.Text.Encoding.Unicode.GetBytes(password));
            //var hashedPassword = System.Text.ASCIIEncoding.Unicode.GetString(sha1data);
            //return hashedPassword;

            //Create the salt value with a cryptographic PRNG:
            byte[] salt;
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            //Create the Rfc2898DeriveBytes and get the hash value:
            var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            //Combine the salt and password bytes for later use:
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            //Turn the combined salt+hash into a string for storage
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        /// <summary>
        /// Verify the user - entered password against a stored password
        /// </summary>
        /// <returns></returns>
        public static bool VerifyPassword(string passwordFromDb, string enteredPassword)
        {
            if (enteredPassword == null || passwordFromDb == null)
                return false;

            //Fetch the stored value
            string savedPasswordHash = passwordFromDb;
            //Extract the bytes 
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            //Get the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            //Compute the hash on the password the user entered 
            var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            //Compare the results
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }
    }
}
