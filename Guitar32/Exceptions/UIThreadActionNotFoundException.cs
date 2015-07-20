using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Exceptions
{
    /// <summary>
    /// Exception thrown when a UIThread action was called but does not exist
    /// </summary>
    public class UIThreadActionNotFoundException : Exception
    {

        /// <summary>
        /// Instantiate an instance of UIThreadActionNotFoundException
        /// </summary>
        /// <param name="actionName">The name of the UIThread action called</param>
        public UIThreadActionNotFoundException(String actionName)
            : base(string.Format("UIThread action \"{0}\" was not found", actionName))
        { }

    }
}
