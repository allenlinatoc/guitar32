using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32
{
    /// <summary>
    /// Class used to contain form data
    /// </summary>
    public class FormData : Dictionary<string, object>
    {

        public FormData()
            : base() { }
        public FormData(IDictionary<string, object> formData)
            : base(formData) { }

    }
}
