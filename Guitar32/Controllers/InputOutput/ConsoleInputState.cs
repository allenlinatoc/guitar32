using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Guitar32.Cryptography;
using Guitar32.Cryptography.Encryption;
using Guitar32.Utilities;

namespace Guitar32.Controllers.InputOutput
{
    /// <summary>
    /// Object containing data and be saved as input state file
    /// </summary>
    [Serializable]
    public class ConsoleInputState : ListState<InputStateArgument>
    {

        private FileInfo fileInfo;
        private string targetModule;

        /// <summary>
        /// Create new ConsoleInputState instance
        /// </summary>
        /// <param name="fileInfo">FileInfo of this input state</param>
        /// <param name="targetModule">Target module's name</param>
        public ConsoleInputState(FileInfo fileInfo, string targetModule)
        {
            if (fileInfo.Exists)
                fileInfo.Delete();

            this.fileInfo = fileInfo;
            this.targetModule = targetModule;
        }

        /// <summary>
        /// Get the FileInfo of this Input state
        /// </summary>
        /// <returns></returns>
        public FileInfo GetFileInfo()
        {
            return fileInfo;
        }

        /// <summary>
        /// Get the array of input state arguments
        /// </summary>
        /// <returns></returns>
        public InputStateArgument[] GetInputStateArguments()
        {
            return this.ToArray();
        }

        /// <summary>
        /// Get the target module's name of this Input State
        /// </summary>
        /// <returns></returns>
        public string GetTargetModule()
        {
            return targetModule;
        }

        public new string Save(KeyIVSignature signature)
        {
            ObjectSerializer.SerializeToFile(GetFileInfo().FullName, this, true);

            // Encrypt
            if (!Rijndael.EncryptFile(GetFileInfo().FullName, signature.GetKey(), signature.GetIV()))
                throw new Exception("Failed to encrypt file: " + GetFileInfo().FullName);

            return GetFileInfo().FullName;
        }


        static public new ConsoleInputState CreateFromFile(FileInfo fileInfo, KeyIVSignature signature)
        {
            return ObjectSerializer.DeserializeFromFile<ConsoleInputState>(fileInfo.FullName);
        }
        


    }
}
