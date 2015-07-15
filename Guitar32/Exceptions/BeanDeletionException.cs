using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Exceptions
{
    /// <summary>
    /// Exception thrown when an attempt to delete a bean class failed
    /// </summary>
    public class BeanDeletionException : Exception
    {

        /// <summary>
        /// Construct new instance of BeanDeletionException
        /// </summary>
        public BeanDeletionException()
            : base("An attempt to delete a bean class failed")
        { }


    }
}
