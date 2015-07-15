using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Exceptions
{
    /// <summary>
    /// Exception thrown when no Runnable Action object can be called
    /// </summary>
    public class NoRunnableException : Exception
    {
        /// <summary>
        /// Construct new instance of NoRunnableException
        /// </summary>
        public NoRunnableException()
            : base("No Runnable Action is callable")
        { }
    }
}
