using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Guitar32
{
    /// <summary>
    /// A serializable list
    /// </summary>
    /// <typeparam name="T">The type of objects that can be enumerated in this list</typeparam>
    [Serializable]
    public class ListState<T> : List<T>
    {

        /// <summary>
        /// Construct new instance of ListState
        /// </summary>
        public ListState() : base()
        { }

        /// <summary>
        /// Construct new instance of ListState from an existing one
        /// </summary>
        /// <param name="list">An existing list object</param>
        public ListState(IEnumerable<T> list)
            : base(list)
        { }

        /// <summary>
        /// Save this ListState object into file
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="key">(Optional) Encryption key</param>
        /// <param name="iv">(Optional) Encryption initialization vector</param>
        /// <param name="filesystemEncrypt">(Optional) Boolean value if file should be encrypted in Filesystem or not</param>
        /// <returns></returns>
        public string Save(string path, byte[] key = null, byte[] iv = null, bool filesystemEncrypt = false)
        {
            Utilities.ObjectSerializer.SerializeToFile(path, this);
            if (key != null && iv != null)
            {
                if (!Cryptography.Encryption.Rijndael.EncryptFile(path, key, iv))
                    throw new Exception("Failed to encrypt file: " + path);

                if (filesystemEncrypt)
                {
                    // Try to encrypt the file
                    FileInfo fileInfo = new FileInfo(path);
                    try
                    {
                        fileInfo.Encrypt();
                    }
                    catch
                    {
                        Utilities.Diagnostics diagnostics = new Utilities.Diagnostics();
                        diagnostics.LogEntry("Failed to encrypt file: " + fileInfo.FullName);
                    }
                }
            }
            return path;
        }

        /// <summary>
        /// Create a ListState from file
        /// </summary>
        /// <param name="path">The path to the source file</param>
        /// <param name="filesystemEncrypt">(Optional) Boolean value if file should be encrypted in Filesystem or not</param>
        /// <param name="key">(Optional) Decryption key</param>
        /// <param name="iv">(Optional) Decryption initialization vector</param>
        /// <returns></returns>
        static public ListState<T> CreateFromFile(string path, bool filesystemDecrypt = false, byte[] key = null, byte[] iv = null)
        {
            if (filesystemDecrypt)
            {
                FileInfo fileInfo = new FileInfo(path);

                // Try to decrypt the file
                try
                {
                    fileInfo.Decrypt();
                }
                catch
                {
                    Utilities.Diagnostics diagnostics = new Utilities.Diagnostics();
                    diagnostics.LogEntry("Failed to decrypt file: " + fileInfo.FullName);
                };
            }

            // Check if Key/IV intel for decryption are provided
            if (key != null && iv != null)
                if (!Cryptography.Encryption.Rijndael.DecryptFile(path, key, iv))
                    throw new Exception("Failed to decrypt file: " + path);

            return Utilities.ObjectSerializer.DeserializeFromFile<ListState<T>>(path);
        }
        

    }
}
