using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Guitar32.Logging
{
    public class Logfile : System.IO.FileStream
    {

        private System.DateTime start;
        private System.DateTime end;

        private string _directoryName;
        private string _filename;


        public Logfile(FileInfo file, bool overwrite = false) 
            : base(file.FullName, overwrite ? FileMode.Create : FileMode.OpenOrCreate)
        {
            _filename = file.Name;
            _directoryName = file.DirectoryName;
        }

        //public Logfile(DirectoryInfo directory)
        //{
        //    _directoryName = directory.FullName;
        //}

    }

}