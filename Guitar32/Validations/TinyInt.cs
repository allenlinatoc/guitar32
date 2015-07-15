using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Guitar32.Validations
{
    public class TinyInt : Validator, Common.IStringDatatype
    {

        public const int MAX_LENGTH = 1;
        public const int MIN_LENGTH = 1;
        private string value;
        static public string expression = @"^(1|0)$";
        static public string message = "Not a valid TinyInt value";


        public TinyInt(string value, bool throwException = false)
        {
            this.value = value;

            if (!isWithinRange())
                throw new Exceptions.OutOfRangeLengthException();
            if (throwException && !this.isValid())
                throw new InvalidTinyIntException();
        }

        public TinyInt(bool value)
        {
            this.value = value ? "1" : "0";
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

        public bool getBooleanValue()
        {
            return this.value == "1";
        }

        public bool isWithinRange()
        {
            int length = this.value.Length;
            return length >= MIN_LENGTH && length <= MAX_LENGTH;
        }

        public override bool isValid()
        {
            return Regex.IsMatch(this.getValue(), expression);
        }

    }

    public class InvalidTinyIntException : Exception
    {
        public InvalidTinyIntException()
            : base(TinyInt.message)
        { }
    }
}
