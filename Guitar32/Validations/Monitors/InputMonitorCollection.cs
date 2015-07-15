using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Guitar32.Validations.Monitors
{
    public class InputMonitorCollection : List<InputMonitor>
    {

        /// <summary>
        /// Initialize an instance of InputMonitorCollection
        /// </summary>
        public InputMonitorCollection() : base() { }


        /// <summary>
        /// Check if every InputMonitor in this collection is ready for user-defined submission
        /// </summary>
        /// <param name="textboxes">Textboxes which should be included in the filter of submission checking</param>
        /// <returns>If this input monitor collection is submittable or not</returns>
        public bool IsSubmittable(params System.Windows.Forms.TextBox[] textboxes) {
            bool enable_filtering = textboxes.Length > 0;
            foreach (InputMonitor monitor in this) {
                if (enable_filtering && !textboxes.Contains(((System.Windows.Forms.TextBox)monitor.GetControl()))) {
                    continue;
                }
                if (!monitor.Validate()) {
                    return false;
                }
            }
            return true;
        }
    }
}
