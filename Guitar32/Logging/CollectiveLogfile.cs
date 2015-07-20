using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Guitar32.Logging
{
    public class CollectiveLogfile
    {
        private List<Logfile> _logfiles;

        public CollectiveLogfile(DirectoryInfo directory)
        {
            if (directory.Exists)
                directory.Create();

            _logfiles = new List<Logfile>();
        }

        //static public List<Logfile> ScanDirectory(DirectoryInfo directory)
        //{
        //    if (!directory.Exists)
        //        throw new IOException("Directory \"" + directory.FullName + "\" does not exist");

        //    // Enumerate files
        //    IEnumerable<FileInfo> files = directory.EnumerateFiles("*.log", SearchOption.TopDirectoryOnly);

        //    foreach (FileInfo file in files)
        //    {
        //        using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
        //        {
                    
        //        }
        //    }
        //}

    }
}
