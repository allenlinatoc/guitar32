using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace Guitar32.Database
{
    [Serializable] public class DatabaseCredentials
    {
        protected String
            database,
            password,
            server,
            username
        ;
        protected uint port;
        protected bool loadsuccess;

        public DatabaseCredentials()
        {
            this.loadsuccess = true;
        }

        /// <summary>
        /// Instantiate from a dump file
        /// </summary>
        /// <param name="path">Path to the dump file</param>
        /// <param name="isFileEncrypted">(Optional) Boolean value if source dump file is encrypted</param>
        /// <param name="key">(Optional) The password/key used to encrypt the encrypted file</param>
        /// <param name="iv">(Optional) The IV used to encrypt the encrypted file</param>
        public DatabaseCredentials(String path, bool isFileEncrypted = false, byte[] key = null, byte[] iv = null) {
            // Check if file exists
            if (!File.Exists(path))     throw new FileNotFoundException();
            else                        File.Decrypt(path);

            // Start
            FileStream fs = null;
            BinaryReader binReader = null;
            try {
                // Decrypt first the file if it is encrypted
                if (isFileEncrypted)
                {
                    if (key == null)
                        throw new ArgumentNullException("key");
                    // Try to decrypt
                    if (!Cryptography.Encryption.Rijndael.DecryptFile(path, key, iv))
                        throw new Exception("Failed to decrypt a file specified as \"Encrypted\"");
                }

                fs = new FileStream(path, FileMode.Open);
                byte[] contents = new byte[fs.Length];
                binReader = new BinaryReader(fs);
                binReader.Read(contents, 0, (int)fs.Length);

                DatabaseCredentials newInstance = Deserialize(contents);
                this.username = newInstance.getUsername();
                this.password = newInstance.getPassword();
                this.server = newInstance.getServer();
                this.port = newInstance.getPort();
                this.database = newInstance.getDatabase();
                this.loadsuccess = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show("An exception occured while constructing DatabaseCredentials object from path: " + ex.Message);
                this.loadsuccess = false;
            }
            finally {
                if (fs != null) {
                    fs.Close();
                    fs.Dispose();
                }
                if (binReader != null) {
                    binReader.Close();
                    binReader.Dispose();
                }
                if (isFileEncrypted)
                {
                    if (!Cryptography.Encryption.Rijndael.EncryptFile(path, key, iv))
                        throw new Exception("Failed to encrypt-back the file");
                }
            }
        }


        /// <summary>
        /// Dump this Database credential to a file
        /// </summary>
        /// <param name="path">The path to file where this credential details will be stored</param>
        /// <param name="encrypt">(Optional) Boolean value if resulting dump file should be encrypted</param>
        /// <param name="key">(Optional) The password/key to encrypt the dump file</param>
        /// <param name="iv">(Optional) The IV to encrypt the dump file</param>
        /// <returns></returns>
        public bool Dump(String path, bool encrypt = false, byte[] key = null, byte[] iv = null) {
            FileStream fileStream = null;
            BinaryWriter binWriter = null;
            
            try {
                fileStream = new FileStream(path, FileMode.Create);
                binWriter = new BinaryWriter(fileStream);
                byte[] serialized = Serialize(this);
                binWriter.Write(serialized);
                binWriter.Close();
                fileStream.Close();

                if (encrypt)
                {
                    if (key == null)
                        throw new ArgumentNullException("key");
                    return Cryptography.Encryption.Rijndael.EncryptFile(path, key, iv);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Error while dumping DatabaseCredentials: " + ex.Message);
                if (fileStream != null) {
                    fileStream.Close();
                }
                if (binWriter != null) {
                    binWriter.Close();
                }
                return false;
            }
            return true;
        }


        /// <summary>
        /// Check if loading of database dump file is successful
        /// </summary>
        /// <returns></returns>
        public bool IsLoadSuccess()
        {
            return this.loadsuccess;
        }

        public String getDatabase() {
            return this.database;
        }

        public String getPassword() {
            return this.password;
        }

        public uint getPort() {
            return this.port;
        }

        public String getServer() {
            return this.server;
        }

        public String getUsername() {
            return this.username;
        }

        public DatabaseCredentials setDatabase(String database) {
            this.database = database;
            return this;
        }

        public DatabaseCredentials setPassword(String password) {
            this.password = password;
            return this;
        }

        public DatabaseCredentials setPort(uint port) {
            this.port = port;
            return this;
        }

        public DatabaseCredentials setServer(String server) {
            this.server = server;
            return this;
        }

        public DatabaseCredentials setUsername(String username) {
            this.username = username;
            return this;
        }

        /// <summary>
        /// Create from a dump file
        /// </summary>
        /// <param name="path">The path to dump file</param>
        /// <returns></returns>
        public static DatabaseCredentials createFromFile(String path) {
            return new DatabaseCredentials(path);
        }

        public static DatabaseCredentials Deserialize(byte[] buffer) {
            DatabaseCredentials ret = null;
            MemoryStream ms = new MemoryStream(buffer);
            BinaryFormatter formatter = new BinaryFormatter();
            try {
                ret = (DatabaseCredentials)formatter.Deserialize(ms);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                //throw new InvalidCredentialsFileException();
            }
            ms.Close();
            return ret;
        }

        public static byte[] Serialize(DatabaseCredentials creds) {
            byte[] result;
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, creds);
            result = stream.ToArray();
            stream.Close();
            return result;
        }

    }



    /// <summary>
    /// Exception thrown when a DatabaseCredentials file is not valid
    /// </summary>
    public class InvalidCredentialsFileException : Exception
    {
        public InvalidCredentialsFileException()
            : base("Not a valid credentials file") { }
    }
}
