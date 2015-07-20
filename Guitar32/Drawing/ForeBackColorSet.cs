using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Guitar32.Drawing
{
    /// <summary>
    /// Instance class to store set of foreground and background colors
    /// </summary>
    public class ForeBackColorSet
    {

        public ForeBackColorSet() { }
        public ForeBackColorSet(Color backColor) {
            this.BackColor = backColor;
        }
        public ForeBackColorSet(Color backColor, Color foreColor) {
            this.BackColor = backColor;
            this.ForeColor = foreColor;
        }

        /// <summary>
        /// Get or set the background color of this Color set
        /// </summary>
        public Color BackColor { get; set; }

        /// <summary>
        /// Get or set the foreground color of this color set
        /// </summary>
        public Color ForeColor { get; set; }


    }
}
