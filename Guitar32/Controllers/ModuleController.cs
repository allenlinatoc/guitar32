using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Guitar32.Cryptography;
using Guitar32.Controllers.InputOutput;

namespace Guitar32.Controllers
{
    public class ModuleController
    {

        public const string DEFAULT_EXPECTED_OUTPUT = "0";

        private FileInfo executableFile;
        private ConsoleInputState consoleInputState;
        private string hash;


        public ModuleController(string targetModule, FileInfo executableConsole, KeyIVSignature signature)
        {
            if (!executableConsole.Exists)
                throw new IOException(string.Format("Executable file \"{0}\" does not exist", executableConsole.FullName));

            this.executableFile = executableConsole;

            // Generate hash, this will server as the the INPUT FILE name
            this.hash = Cryptography.MD5Hash.GenerateRandom();
            string _inputFilePath = executableConsole.DirectoryName + "\\" + this.hash + ".IN";

            // Initialize ConsoleInputState file
            this.consoleInputState = new ConsoleInputState(new FileInfo(_inputFilePath), targetModule);
        }

        /// <summary>
        /// Get the generated hash string for this controller
        /// </summary>
        /// <returns></returns>
        public string GetHash()
        {
            return this.hash;
        }

        /// <summary>
        /// Get the path of InputFile
        /// </summary>
        /// <returns></returns>
        public ConsoleInputState GetConsoleInputState()
        {
            return consoleInputState;
        }

        public bool Execute(string expectedOutput = DEFAULT_EXPECTED_OUTPUT, bool debugMode = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(this.executableFile.FullName, GetConsoleInputState().GetFileInfo().FullName);
            startInfo.LoadUserProfile = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            Process process = new Process();
            process.StartInfo = startInfo;

            // Launch executable console
            process.Start();
            string output = process.StandardOutput.ReadToEnd().Trim();

            process.WaitForExit();
            if (debugMode)
                Console.WriteLine("Module [" + GetConsoleInputState().GetTargetModule() + "]: " + output);
            
            bool success = (expectedOutput == output);

            // Cleanup if success
            if (success)
            {
                // If INPUT file still exist, try to delete it
                FileInfo inputFileInfo = this.GetConsoleInputState().GetFileInfo();
                if (inputFileInfo.Exists)
                {
                    try { inputFileInfo.Decrypt(); }
                    catch { };
                    inputFileInfo.Delete();
                }
                
                // If OUTPUT file still exist, try to delete it
                FileInfo outputFileInfo =
                    new FileInfo(inputFileInfo.DirectoryName + "\\" + this.GetHash() + ".OUT");
                if (outputFileInfo.Exists)
                {
                    try { outputFileInfo.Decrypt(); }
                    catch { };
                    outputFileInfo.Delete();
                }
            }

            return success;
        }


    }
}
