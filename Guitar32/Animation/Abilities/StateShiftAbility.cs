using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Guitar32.Animation.Components;

namespace Guitar32.Animation.Abilities
{
    public class StateShiftAbility : Ability
    {


        public StateShiftAbility(Control control, String[] events) : base(control)
        {
            if (events.Contains("CLICK"))
            {
                this.getControl().Click += StateShiftAbility_Click;
            }
        }

        void StateShiftAbility_Click(object sender, EventArgs e)
        {

        }



    }
}
