using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Guitar32.Common
{
    /// <summary>
    /// Abstract class for class with UI Control binding
    /// </summary>
    /// <typeparam name="T">The type of Control to be bound</typeparam>
    public class AbstractControlBinder<T>
    {
        protected T control;


        /// <summary>
        /// Get the control associated with this binder
        /// </summary>
        /// <typeparam name="T">The control to be bound</typeparam>
        public T getControl()
        {
            return this.control;
        }

        /// <summary>
        /// Associate a control to this binder
        /// </summary>
        /// <param name="control">The Control to be associated to this binder</param>
        protected void setControl(T control) {
            this.control = control;
        }

    }
}
