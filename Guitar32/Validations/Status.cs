using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Guitar32;
using Guitar32.Common;

namespace Guitar32.Validations
{
    public class Status : Validator, IStringDatatype
    {

        public const string ACTIVE = "ACTIVE";
        public const string INACTIVE = "INACTIVE";

        public const int MAX_LENGTH = 8;
        public const int MIN_LENGTH = 6;
        static public string expression = "^(ACTIVE|INACTIVE)$";
        static public string message = "Allowed `Status` values are \"ACTIVE\" or \"INACTIVE\"";
        private string value;

        public Status(string value, bool throwException = false)
        {
            this.value = value.ToUpper();
            if (!this.isWithinRange())
                throw new Guitar32.Exceptions.OutOfRangeLengthException();

            if (throwException && !this.isValid())
                throw new InvalidStatusException();
        }

        public override bool isValid()
        {
            return Regex.IsMatch(this.getValue(), expression, RegexOptions.IgnoreCase);
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
    }

    public class InvalidStatusException : Exception
    {
        public InvalidStatusException()
            : base(Status.message)
        { }
    }

}
