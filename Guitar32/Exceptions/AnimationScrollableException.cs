using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Exceptions
{
    /// <summary>
    /// Exception thrown when a control provided is not scrollable
    /// </summary>
    public class AnimationScrollableException : Exception
    {

        /// <summary>
        /// Instantiate an instance of AnimationScrollableException
        /// </summary>
        public AnimationScrollableException()
            : base("Provided control is not scrollable")
        { }

    }
}
