using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Exceptions
{
    /// <summary>
    /// Exception thrown when specified control didn't mathc with the current control in instance
    /// </summary>
    public class ControlMatchException : Exception
    {

        /// <summary>
        /// Instantiate an instance of ControlMatchException
        /// </summary>
        public ControlMatchException() :
            base("Specified control didn't match with the current control in instance")
        { }

    }
}
