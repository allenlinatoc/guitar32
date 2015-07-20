using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.IO;



namespace Guitar32.Cryptography.Encryption
{
    /// <summary>
    /// Utility class for Rijndael encryption.
    /// Refer to: https://msdn.microsoft.com/en-us/library/system.security.cryptography.rijndaelmanaged(v=VS.90).aspx for more information
    /// </summary>
    public class Rijndael
    {

        /// <summary>
        /// Encrypt a file using Rijndael
        /// </summary>
        /// <param name="path">The path to the file which will be encrypted</param>
        /// <param name="password">The password to encrypt the file</param>
        /// <returns></returns>
        static public bool EncryptFile(String path, byte[] key, byte[] iv)
        {
            try
            {
                String outputFile = path.TrimEnd('.') + ".tmp";

                UnicodeEncoding UE = new UnicodeEncoding();

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, iv),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(path, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();

                File.Delete(path);
                File.Move(outputFile, path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Decrypt an encrypted file using Rijndael
        /// </summary>
        /// <param name="path">The path of the file to be decrypted</param>
        /// <param name="password">The password which will be used to decrypt the file</param>
        /// <returns></returns>
        static public bool DecryptFile(String path, byte[] key, byte[] iv)
        {

            try
            {
                String outputFile = path.TrimEnd('.') + ".tmp";

                UnicodeEncoding UE = new UnicodeEncoding();

                FileStream fsCrypt = new FileStream(path, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, iv),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();

                File.Delete(path);
                File.Move(outputFile, path);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }



        /// <summary>
        /// Encrypt a string to bytes
        /// </summary>
        /// <param name="plainText">The string to be encrypted</param>
        /// <param name="Key">The shared key that will be used for encryption process</param>
        /// <param name="IV">The initialization vector</param>
        /// <returns></returns>
        static public byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }


        /// <summary>
        /// Decrypt an encrypted series of bytes into string
        /// </summary>
        /// <param name="cipherText">The byte cipher to be decrypted</param>
        /// <param name="Key">The secret key used to encrypt the ciphered bytes</param>
        /// <param name="IV">The initialization vector used to encrypt the ciphered bytes</param>
        /// <returns></returns>
        static public string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold 
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

    }

}
