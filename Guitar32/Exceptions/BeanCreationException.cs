using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Exceptions
{
    /// <summary>
    /// Exception throw when a failure occured during creation of a bean class
    /// </summary>
    public class BeanCreationException : Exception
    {
        /// <summary>
        /// Construct new instance of BeanCreationException
        /// </summary>
        public BeanCreationException()
            : base("Failed to create bean data")
        { }
    }
}
