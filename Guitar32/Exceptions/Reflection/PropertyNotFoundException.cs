using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Exceptions.Reflection
{

    /// <summary>
    /// Exception thrown when trying to dynamically access/set a property from an object
    /// </summary>
    public class PropertyNotFoundException : Exception
    {

        /// <summary>
        /// Instantiate a new instance of PropertyNotFoundException
        /// </summary>
        /// <param name="propertyName">The name of the specified property</param>
        public PropertyNotFoundException(String propertyName)
            : base(string.Format("Property \"{0}\" does not exist", propertyName))
        { }


    }
}
