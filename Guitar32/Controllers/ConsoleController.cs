using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Guitar32.Controllers.InputOutput;
using Guitar32.Cryptography;
using Guitar32.Utilities;

namespace Guitar32.Controllers
{
    public class ConsoleController
    {

        const string PARAM_EXPRESSION = @"^\-([A-Za-z]{1}[\w]+)\(.+\)$";

        protected Dictionary<string, System.Delegate> actions;
        protected string consoleArgument;
        protected string lastMatchingAction;
        private string moduleName;
        private SystemResponse moduleResponse = null;
        private bool acceptsConsoleCommand = false;
        private bool isEvaluated = false;
        private object evaluationResult = null;
        private KeyIVSignature signature;


        public ConsoleController(string moduleName, string[] consoleArguments, KeyIVSignature signature, bool acceptsConsoleCommand = false)
        {
            this.actions = new Dictionary<string, Delegate>();
            this.lastMatchingAction = null;
            this.moduleName = moduleName;
            this.signature = signature;
            this.acceptsConsoleCommand = acceptsConsoleCommand;
            this.consoleArgument = string.Join(" ", consoleArguments);
        }

        public void AssignDelegate(string actionName, System.Delegate method)
        {
            this.actions.Add(actionName.ToUpper(), method);
        }

        public void RemoveDelegate(string actionName)
        {
            if (this.actions.ContainsKey(actionName.ToUpper()))
                this.actions.Remove(actionName.ToUpper());
        }

        public object Evaluate(bool suppressReturnCode = false)
        {
            if (IsEvaluated())
                return evaluationResult;

            ReturnCode returnCode = ReturnCode.FAILED;

            string argument = this.GetConsoleArgument();
            object result = null;

            // Check if argument is a parameter command
            if (this.AcceptsConsoleCommand())
            {
                if (Regex.IsMatch(argument, PARAM_EXPRESSION))
                {
                    // If it is a parameter command, get the delegate name
                    string delegateName = argument.Substring(1, argument.IndexOf('(') - 1).ToUpper();
                    string[] parameters = Regex.Match(argument, @"\(.+\)").Value.Trim('(').Trim(')').Replace(",", "{comma_escaped}").Split(',');

                    if (this.actions.ContainsKey(delegateName))
                    {
                        System.Delegate delegateMethod = this.actions[delegateName];

                        // Get parameter count
                        int parameterCount = delegateMethod.Method.GetParameters().Length;

                        // Formalize parameters
                        List<object> listParameters = new List<object>();
                        foreach (string parameter in parameters)
                            listParameters.Add(parameter.Replace("{comma_escaped}", ","));

                        // Invoke method
                        result = delegateMethod.DynamicInvoke(listParameters.ToArray());
                    }
                    else
                        returnCode = ReturnCode.UNEQUAL_ACTIONNAME;
                }
                if (result != null)
                    returnCode = ReturnCode.SUCCESS;
            }
            else
            {
                // Check if this argument is a file
                try
                {
                    FileInfo fileInfo = new FileInfo(this.GetConsoleArgument());

                    // Check if this file exists
                    if (fileInfo.Exists)
                    {
                        // Try to deserialize
                        ConsoleInputState inputState = ConsoleInputState.CreateFromFile(new FileInfo(fileInfo.FullName), this.signature);

                        // After successful deserialization,
                        //  check if target module matches
                        if (inputState.GetTargetModule().Trim().ToLower() == this.GetModuleName().Trim().ToLower())
                        {
                            InputStateArgument[] inputStateArguments = inputState.ToArray();

                            foreach (InputStateArgument inputArgument in inputStateArguments)
                            {
                                // check if target Action matches
                                string actionName = inputArgument.GetActionName();

                                if (this.actions.ContainsKey(actionName))
                                {
                                    this.actions[actionName].DynamicInvoke(inputArgument.GetParameters());
                                }
                                else
                                    returnCode = ReturnCode.UNEQUAL_ACTIONNAME;
                            }
                        }
                        else
                            returnCode = ReturnCode.UNEQUAL_MODULENAME;
                    }
                }
                catch (Exception ex)
                {
                    // do nothing, `streamOutput` remains "-1"
                }
            }

            if (!suppressReturnCode)
            {
                // Write to output stream
                Console.WriteLine(returnCode.ToString());
            }
            isEvaluated = true;
            return this.evaluationResult;
        }

        public bool AcceptsConsoleCommand()
        {
            return this.acceptsConsoleCommand;
        }

        public string GetConsoleArgument()
        {
            return this.consoleArgument;
        }

        public string GetLastMatchingAction()
        {
            return this.lastMatchingAction;
        }

        public string GetModuleName()
        {
            return this.moduleName;
        }

        public SystemResponse GetModuleResponse()
        {
            return this.moduleResponse;
        }

        public bool IsEvaluated()
        {
            return this.isEvaluated;
        }

        
        //
        // User-defined methods
        //

        public void LogError(string errorMessage)
        {
            Console.WriteLine(
                string.Format("{0} [ERROR]\t{1}"
                    , Guitar32.Validations.DateTime.CreateFromNativeDateTime(System.DateTime.Now).getValue()
                    , errorMessage));
        }

        public void LogMessage(string message)
        {
            Console.WriteLine(
                string.Format("{0} [NOTICE]\t{1}"
                    , Guitar32.Validations.DateTime.CreateFromNativeDateTime(System.DateTime.Now).getValue()
                    , message));
        }

        public void SetModuleResponse(SystemResponse moduleResponse)
        {
            this.moduleResponse = moduleResponse;
        }

        
        //
        // Private methods
        //

        private object _evaluateConsoleCommand(string consoleCommand)
        {
            object result = null;
            
            return result;
        }


        public enum ReturnCode : int
        {
            FAILED = -1,
            SUCCESS = 0,
            UNEQUAL_MODULENAME = 1,
            UNEQUAL_ACTIONNAME = 2
        }


    }
}
