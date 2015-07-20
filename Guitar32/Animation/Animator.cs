using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Guitar32.Common;

namespace Guitar32.Animation
{
    /// <summary>
    /// Class used to store animation abilities for a control
    /// </summary>
    public class Animator : List<Ability>
    {
        private Control control;

        /// <summary>
        /// Instantiate a new instance of Animator
        /// </summary>
        /// <param name="control"></param>
        public Animator(Control control)
        {
            this.control = control;
        }

        /// <summary>
        /// Adds an animation ability to this animation
        /// </summary>
        /// <param name="ability">The animation ability to be added</param>
        public new void Add(Ability ability)
        {
            if (ability.getControl() != control)
            {
                throw new Exceptions.ControlMatchException();
            }
            base.Add(ability);
        }


        /// <summary>
        /// Adds an animation ability to this animation (Alias of Add)
        /// </summary>
        /// <param name="ability">The animation ability to be added</param>
        public void AddAbility(Ability ability)
        {
            this.Add(ability);
        }


        /// <summary>
        /// Copy the abilities of other animator to its set of abilities
        /// </summary>
        /// <param name="animator">The animator which abilities will be copied from</param>
        public void CopyAbilitiesFrom(Animator animator)
        {
            foreach (Ability ability in animator)
            {
                if (!this.Contains(ability))
                {
                    this.Add(ability);
                }
            }
        }
        


    }
}
