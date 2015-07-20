using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Guitar32.Animation.Abilities
{
    /// <summary>
    /// An animation ability to enable smooth scrolling to scrollable controls
    /// </summary>
    public class SmoothScrollAbility : Guitar32.Animation.Ability
    {

        private Timer timer;
        private int scrollto;
        private ScrollOrientation scrollorientation;
        int startvalue = -1;
        int speed = 0;


        /// <summary>
        /// Instantiate a new instance of SmoothScrollAbility
        /// </summary>
        /// <param name="control">The control to be bound</param>
        public SmoothScrollAbility(Control control, ScrollOrientation orientation) : base(control)
        {
            // Check if this control is scrollable
            System.Type ctrlType = control.GetType();
            if (!ctrlType.BaseType.IsSubclassOf(typeof(ScrollableControl)) && !ctrlType.BaseType.IsSubclassOf(typeof(ContainerControl)))
            {
                throw new Guitar32.Exceptions.AnimationScrollableException();
            }

            // Initialize timer
            this.timer = new Timer();
            this.timer.Interval = 10;
            this.timer.Tick += new EventHandler(timer_Tick);
            this.scrollorientation = orientation;
            this.scrollto = this.scrollorientation == ScrollOrientation.VerticalScroll ?
                this.getControl().VerticalScroll.Value : this.getControl().HorizontalScroll.Value;
            this.timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (this.scrollto != this.getScrollValue())
            {
                int coefficient = this.scrollto > this.getScrollValue() ? 1 : -1;
                if (this.startvalue == -1)
                {
                    this.startvalue = this.getScrollValue();
                    this.speed = 1;
                }
                this.ScrollRun(this.speed * coefficient);
                // Check for speed stall
                this.speed += this.getScrollValue() > (this.scrollto / 2) ? 1 : -1;
                return;
            }
            this.startvalue = -1;
            this.speed = 0;
            this.timer.Stop();
        }


        /// <summary>
        /// Get the control associated in this animation ability
        /// </summary>
        /// <returns></returns>
        public new ScrollableControl getControl()
        {
            return (ScrollableControl)base.getControl();
        }

        public ScrollOrientation getScrollOrientation()
        {
            return this.scrollorientation;
        }

        public int getScrollValue()
        {
            return this.getScrollOrientation() == ScrollOrientation.VerticalScroll ?
                this.getControl().VerticalScroll.Value : this.getControl().HorizontalScroll.Value;
        }


        public void ScrollTo(int point)
        {

        }

        public void ScrollRun(int amount)
        {
            // Check if "scrollto" has value
            if (this.scrollto == -1)
                return;

            // Prevent overflow
            amount = (this.getScrollValue() + amount) > this.scrollto ? this.scrollto - this.getScrollValue() : amount;
            // Proceed
            if (this.getScrollOrientation() == ScrollOrientation.VerticalScroll)
            {
                this.getControl().VerticalScroll.Value += amount;
                return;
            }
            this.getControl().HorizontalScroll.Value += amount;
        }


    }
}
