using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Guitar32.Animation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Ability : Guitar32.Common.AbstractControlBinder<Control>
    {

        /// <summary>
        /// Instantiate an instance of this animation ability
        /// </summary>
        /// <param name="control">The control to be bound</param>
        public Ability(Control control)
        {
            this.setControl(control);
        }


    }
}
