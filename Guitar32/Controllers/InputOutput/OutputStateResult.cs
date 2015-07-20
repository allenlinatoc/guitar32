using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Controllers.InputOutput
{
    public class OutputStateResult
    {

        private string sourceAction;
        private string code;
        private object data;

        public OutputStateResult(string sourceAction, string code, object data)
        {
            this.sourceAction = sourceAction;
            this.code = code.Trim().ToUpper();
            this.data = data;
        }

        public string GetSourceAction()
        {
            return this.sourceAction;
        }

        public string GetCode()
        {
            return this.code;
        }

        public object GetData()
        {
            return this.data;
        }


    }
}
