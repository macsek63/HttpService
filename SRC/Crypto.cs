using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Security.Cryptography;


namespace HttpService
{
	public class Crypto
	{

        //private TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
        //private MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();

        static private byte[] MD5Hash(string value)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            return MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(value));
        }

        static public string Encrypt(string stringToEncrypt, string key)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            DES.Key = MD5Hash(key);
            DES.Mode = CipherMode.ECB;
            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(stringToEncrypt);
            return Convert.ToBase64String(DES.CreateEncryptor().TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        static public string Decrypt(string encryptedString, string key)
        {
            try
            {
                TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
                DES.Key = MD5Hash(key);
                DES.Mode = CipherMode.ECB;
                byte[] Buffer = Convert.FromBase64String(encryptedString);
                Buffer = DES.CreateDecryptor().TransformFinalBlock(Buffer, 0, Buffer.Length);
                return ASCIIEncoding.ASCII.GetString(Buffer, 0, Buffer.Length);
            }
            catch 
            {
                MessageBox.Show("B³¹d deszyfracji", "B³¹d deszyfracji");
                return "";
            }
        }
    }
	
	
}
