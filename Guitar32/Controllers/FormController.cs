using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Guitar32.Validations.Monitors;

namespace Guitar32.Controllers
{
    /// <summary>
    /// Base class to enable Form controller operations
    /// </summary>
    public class FormController : Form
    {
        private InputMonitorCollection inputMonitorCollection;
        private bool detectCloseOperations = false;
        private bool is_surface_draggable = false;
        private bool is_canvas_focusable = false;
        private Color? focuscolor = null, lostfocuscolor = null;
        private FormData formData;
        // Draggability values
        private bool is_mousedown;
        private Size offsetDistance = new Size();

        
        /// <summary>
        /// Instantiate this FormController
        /// </summary>
        /// <param name="detectCloseOperations">(Optional) If close operations should be detected (e.g. unsaved user inputs detection, etc.)</param>
        public FormController(bool detectCloseOperations = false) : base() {
            this.inputMonitorCollection = new InputMonitorCollection();
            this.detectCloseOperations = detectCloseOperations;
            this.FormClosing += new FormClosingEventHandler(FormController_FormClosing);
            
            // Set mouse events
            this.MouseDown += new MouseEventHandler(FormController_MouseDown);
            this.MouseUp += new MouseEventHandler(FormController_MouseUp);
            this.MouseMove += new MouseEventHandler(FormController_MouseMove);
            this.Activated += FormController_Activated;
            this.Deactivate += FormController_Deactivate;
        }

        void FormController_Activated(object sender, EventArgs e)
        {
            if (this.is_canvas_focusable)
            {
                this.BackColor = (Color)this.focuscolor;
            }
        }

        void FormController_Deactivate(object sender, EventArgs e)
        {
            if (this.is_canvas_focusable)
            {
                this.BackColor = (Color)this.lostfocuscolor;
            }
        }

        void FormController_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.is_mousedown && this.is_surface_draggable)
            {
                Point screenLocation = this.PointToScreen(new Point(0, 0));
                this.SetDesktopLocation(Cursor.Position.X - offsetDistance.Width, Cursor.Position.Y - offsetDistance.Height);
            }
        }

        void FormController_MouseDown(object sender, MouseEventArgs e)
        {
            this.is_mousedown = true;
            Point screenLocation = this.GetScreenLocation();
            this.offsetDistance = new Size(Point.Subtract(Cursor.Position, new Size(screenLocation.X, screenLocation.Y)));
        }

        void FormController_MouseUp(object sender, MouseEventArgs e)
        {
            this.is_mousedown = false;
        }


        void FormController_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.detectCloseOperations) {
                if (this.HasUnsavedChanges()) {
                    DialogResult dResult = MessageBox.Show("You have unsaved changes. Are you sure you want to close this form?", "Confirm form closing", MessageBoxButtons.YesNo);
                    if (dResult != DialogResult.Yes) {
                        e.Cancel = true;
                    }
                }
            }
        }


        /// <summary>
        /// Add an input monitor in one this form's input fields
        /// </summary>
        /// <param name="inputMonitor">The InputMonitor object to be added</param>
        public FormController AddInputMonitor(InputMonitor inputMonitor)
        {
            this.inputMonitorCollection.Add(inputMonitor);
            return this;
        }
        /// <summary>
        /// Add an input monitor in one this form's input fields
        /// </summary>
        /// <param name="inputMonitors">Array of/Variable input monitors to be added</param>
        public void AddInputMonitor(params InputMonitor[] inputMonitors)
        {
            foreach (InputMonitor monitor in inputMonitors)
            {
                this.AddInputMonitor(monitor);
            }
        }


        /// <summary>
        /// Disable the whole form
        /// </summary>
        public void Disable() {
            this.Enabled = false;
        }


        /// <summary>
        /// Disable detection of onClose operations
        /// </summary>
        public void DisableCloseDetections() {
            if (this.detectCloseOperations) {
                this.detectCloseOperations = false;
            }
        }


        /// <summary>
        /// Enable the whole form
        /// </summary>
        public void Enable() {
            this.Enabled = true;
        }


        /// <summary>
        /// Enable detection of onClose operations
        /// </summary>
        public void EnableCloseDetections() {
            if (!this.detectCloseOperations) {
                this.detectCloseOperations = true;
            }
        }


        /// <summary>
        /// Get the FormData object of this form
        /// </summary>
        /// <returns></returns>
        public FormData GetFormData() {
            return this.formData;
        }


        /// <summary>
        /// Get the screen location of this form
        /// </summary>
        /// <returns></returns>
        public Point GetScreenLocation()
        {
            return this.PointToScreen(new Point(0, 0));
        }


        /// <summary>
        /// Get all input monitors in this form
        /// </summary>
        /// <returns></returns>
        public InputMonitorCollection GetInputMonitors() {
            return this.inputMonitorCollection;
        }


        /// <summary>
        /// Check if this form has FormData contents inside
        /// </summary>
        /// <returns></returns>
        public bool HasFormData() {
            return this.formData != null && this.formData.Count > 0;
        }


        /// <summary>
        /// Check if this form has fields with potential pending unsaved changes
        /// </summary>
        /// <param name="controls">Do not supply this parameter!!!</param>
        /// <returns>If this form has fields with potential pending unsaved changes</returns>
        public bool HasUnsavedChanges(Control.ControlCollection controls = null) {
            if (controls == null) {
                controls = this.Controls;
            }
            foreach (Control control in controls) {
                System.Type controlType = control.GetType();
                if (controlType.Equals(typeof(TextBox))) {
                    TextBox textBox = (TextBox)control;
                    if (textBox.Text.Length > 0) {
                        return true;
                    }
                }
                if (control.Controls.Count > 0) {
                    return this.HasUnsavedChanges(control.Controls);
                }
            }

            return false;
        }


        /// <summary>
        /// Check if this form is ready for submission
        /// </summary>
        /// <param name="textboxes">Textboxes which should be included in the filter of submission checking</param>
        /// <returns>If this form is ready for submission</returns>
        public bool IsSubmittable(params TextBox[] textboxes) {
            return this.GetInputMonitors().IsSubmittable(textboxes);
        }


        /// <summary>
        /// Check if this form is surface draggable or not
        /// </summary>
        /// <returns></returns>
        public bool IsSurfaceDraggable()
        {
            return this.is_surface_draggable;
        }


        /// <summary>
        /// Reset all child fields in this form
        /// </summary>
        /// <param name="exceptions">(Optional) An array of controls to be excluded from the reset</param>
        /// <param name="controls">[NO!] Do not supply this parameter!!!</param>
        public void ResetFields(Control[] exceptions = null, Control.ControlCollection controls = null) {
            if (controls == null) {
                controls = this.Controls;
            }
            foreach (Control control in controls) {
                // Check if current control is in exceptions
                if (exceptions != null && exceptions.Contains(control)) {
                    continue;
                }
                // Proceed
                if (control.GetType().Equals(typeof(TextBox))) {
                    TextBox textBox = (TextBox)control;
                    textBox.Clear();
                }
                else if (control.GetType().Equals(typeof(ComboBox))) {
                    ComboBox comboBox = (ComboBox)control;
                    comboBox.SelectedIndex = 0;
                }

                if (control.HasChildren) {
                    ResetFields(exceptions, control.Controls);
                }
            }
        }


        /// <summary>
        /// Set the FormData of this form of this Form
        /// </summary>
        /// <param name="formData">The FormData to be passed to this form</param>
        public void SetFormData(FormData formData) {
            this.formData = formData;
        }


        /// <summary>
        /// Set if this form's canvas can have focus capability or not
        /// <param name="focuscolor">(Optional) Canvas Color to be used when this form is focused</param>
        /// <param name="lostfocuscolor"> </param>
        /// </summary>
        /// <param name="is_canvas_focusable"></param>
        public void SetCanvasFocusable(bool is_canvas_focusable, Color? focuscolor = null, Color? lostfocuscolor = null)
        {
            if (is_canvas_focusable)
            {
                if (focuscolor == null)
                    throw new ArgumentNullException("focuscolor");
                if (lostfocuscolor == null)
                    throw new ArgumentNullException("lostfocuscolor");
            }
            this.is_canvas_focusable = is_canvas_focusable;
            this.focuscolor = focuscolor;
            this.lostfocuscolor = lostfocuscolor;
        }

        
        /// <summary>
        /// Set if this form's surface has draggability or not
        /// </summary>
        /// <param name="isdraggable">Boolean value if this form's surface will have draggability or not</param>
        public void SetSurfaceDraggability(bool isdraggable)
        {
            this.is_surface_draggable = isdraggable;
        }


        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormController
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "FormController";
            this.ResumeLayout(false);

        }


    }
}
