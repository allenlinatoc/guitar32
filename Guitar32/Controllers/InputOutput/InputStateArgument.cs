using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Controllers.InputOutput
{
    public class InputStateArgument
    {

        private string actionName;
        private object[] parameters;


        public InputStateArgument(string actionName, object[] parameters)
        {
            this.actionName = actionName.Trim().ToUpper();
            this.parameters = parameters;
        }

        public string GetActionName()
        {
            return actionName;
        }
        public object[] GetParameters()
        {
            return parameters;
        }

    }
}
