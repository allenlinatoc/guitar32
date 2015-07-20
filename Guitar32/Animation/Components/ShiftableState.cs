using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Guitar32.Common;
using Guitar32.Exceptions.Reflection;

namespace Guitar32.Animation.Components
{
    public class ShiftableState<T> : AbstractControlBinder<Control>
    {
        private string propertyName;
        private Type propertyType;
        private object[] values;
        private object originalValue;
        private int stateCounter;


        public ShiftableState(Control control, string propertyName, object[] values)
        {
            this.setControl(control);
            PropertyInfo[] propertyInfos = control.GetType().GetProperties();
            // Find the match
            foreach (PropertyInfo prop in propertyInfos)
            {
                // Match found
                if (prop.Name.Equals(propertyName))
                {
                    // Get property type
                    propertyType = this.getControl().GetType().GetProperty(propertyName).PropertyType.GetType();

                    List<object> objects = new List<object>();
                    objects.Add(this.getControl().GetType().GetProperty(propertyName).GetValue(this.getControl(), null));

                    // Inspect each value if they have same data-type
                    for (int i = 0; i < values.Length; i++)
                    {
                        object value = values[i];
                        if (propertyType != value.GetType())
                        {
                            throw new Exception(string.Format("One of the supplied values has invalid type: \"{0}\"", value.GetType()));
                        }
                        objects.Add(value);
                    }

                    this.values = objects.ToArray();

                    // Set stateCounter
                    stateCounter = 0;
                    return;
                }
            }
            throw new PropertyNotFoundException(propertyName);
        }


        public new T getControl()
        {
            return (T) Convert.ChangeType(this.control, typeof (T));
        }


        private T getStateAt<T>(int index)
        {
            return (T) Convert.ChangeType(values[index], typeof (T));
        }


        public string getPropertyName()
        {
            return this.propertyName;
        }


        public void ShiftState<T>()
        {
            stateCounter = (stateCounter + 1) % this.values.Length;
            this.getControl().GetType().GetProperty(this.getPropertyName()).SetValue(this.getControl(), getStateAt<T>(stateCounter), null);
        }



    }
}
