using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Exceptions
{
    /// <summary>
    /// Exception thrown when a child bean cannot be created
    /// </summary>
    public class ChildBeanCreationException : Exception
    {
        public ChildBeanCreationException() 
            : base("One of the child beans cannot be created. Make sure all child beans don't exist and valid")
        { }

    }
}
