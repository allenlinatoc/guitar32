using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Exceptions
{
    /// <summary>
    /// Exception thrown when a failure occured while a data is being updated to a data source
    /// </summary>
    public class DataUpdateException : Exception
    {
        public DataUpdateException()
            : base("A data cannot be updated to a data source") { }
    }
}
