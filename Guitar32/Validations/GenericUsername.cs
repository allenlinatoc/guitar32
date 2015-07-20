using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Guitar32.Validations
{
    public class GenericUsername : Validator, Common.IStringDatatype
    {

        const int MIN_LENGTH = 4;
        const int MAX_LENGTH = 25;
        private string value;
        public const string expression = @"^[A-Za-z]{1,2}([A-Za-z0-9][_.]{0,1}){3,23}$";
        public const string message = "Not a valid format of username. Only alphanumeric, periods and underscores are allowed.";

        public GenericUsername(string value, bool throwException = false)
        {
            this.value = value;
            if (!isWithinRange())
                throw new Exceptions.OutOfRangeLengthException();
            if (throwException && !isValid())
                throw new InvalidGenericUsernameException();
        }


        public int getMaxLength()
        {
            return MAX_LENGTH;
        }

        public int getMinLength()
        {
            return MIN_LENGTH;
        }

        public string getValue()
        {
            return this.value;
        }

        public bool isWithinRange()
        {
            int length = this.getValue().Length;
            return length >= MIN_LENGTH && length <= MAX_LENGTH;
        }

        public override bool isValid()
        {
            return Regex.IsMatch(getValue(), expression);
        }
    }

    public class InvalidGenericUsernameException : Exception
    {
        public InvalidGenericUsernameException()
            : base(GenericUsername.message)
        { }
    }

}
