using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Guitar32;
using Guitar32.Common;
using Guitar32.Exceptions;
using Guitar32.Utilities;
using MySql.Data.Types;


namespace Guitar32.Validations
{
    public class DateTime : Validator, IStringDatatype
    {
        public static String expression = "^(([0-9]{2,4})-(0[1-9]|1[0-2])-(0[1-9]|[1-2][0-9]|3[01])(?:\\s(([0-1][0-9])|(2[0-3])):([0-5][0-9]):([0-5][0-9]))?)|((?:(([0-1][0-9])|(2[0-3])):([0-5][0-9]):([0-5][0-9]))?)$";
        //public static String date_expression = @"([0-9]{2,4})-(0[1-9]|1[0-2])-(0[1-9]|[1-2][0-9]|3[01])";
        //public static String time_expression = @"(((([0-1][0-9])|(2[0-3])):([0-5][0-9]):([0-5][0-9]))?)";
        public static String message = "Not a valid Date/Time, please check and try again";
        private String value;
        const int MIN_LENGTH = 10;
        const int MAX_LENGTH = 19;

        public DateTime(String value, bool throwException = false)
        {
            this.value = value;
            if (throwException && value != null) {
                if (!this.isValid()) {
                    throw new InvalidDateTimeException();
                }
                //if (this.getValue().Length > 0) {
                //    if (!this.isWithinRange()) {
                //        throw new Guitar32.Exceptions.OutOfRangeLengthException();
                //    }
                //}
            }
        }

        /// <summary>
        /// Get the Date-only component of this DateTime. Return NULL if no date-component found
        /// </summary>
        /// <returns></returns>
        public String getDateOnly()
        {
            MySqlDateTime dt = new MySqlDateTime(System.DateTime.Parse(this.value));
            return string.Format("{0}-{1}-{2}", Integer.Pad(dt.Year, 4), Integer.Pad(dt.Month, 2), Integer.Pad(dt.Day, 2));
        }


        /// <summary>
        /// Get the Time-only component of this DateTime. Returns NULL if no time-component found
        /// </summary>
        /// <returns></returns>
        public String getTimeOnly()
        {
            MySqlDateTime dt = new MySqlDateTime(System.DateTime.Parse(this.value));
            return string.Format("{0}:{1}:{2}", Integer.Pad(dt.Hour, 2), Integer.Pad(dt.Minute, 2), Integer.Pad(dt.Second, 2));
        }

        public int getMaxLength() {
            return MAX_LENGTH;
        }

        public int getMinLength() {
            return MIN_LENGTH;
        }

        public String getValue() {
            return this.value;
        }

        public bool isWithinRange() {
            return this.getValue().Length >= this.getMinLength() && this.getValue().Length <= this.getMaxLength();
        }

        public override bool isValid() {
            return this.getValue().Length > 0 ?
                Regex.IsMatch(this.getValue(), expression, RegexOptions.IgnoreCase) : true;
        }

        /// <summary>
        /// Get the System.DateTime representation of this DateTime's value
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.FormatException"></exception>
        public System.DateTime ToNativeDateTime()
        {
            return System.DateTime.Parse(this.getValue());
        }



        // <Begin::Static methods>
        
        /// <summary>
        /// Create a DateTime instance from a DateTimePicker control
        /// </summary>
        /// <param name="datetimePicker">The source DateTimePicker control</param>
        /// <param name="includeTime">(Optional) If time should also be included in the result</param>
        /// <returns>The resulting DateTime instance</returns>
        public static DateTime CreateFromDateTimePicker(System.Windows.Forms.DateTimePicker datetimePicker, bool includeTime=false) {
            System.DateTime value = datetimePicker.Value;
            return CreateFromNativeDateTime(value, includeTime);
        }


        /// <summary>
        /// Create a DateTime instance from a native System.DateTime object
        /// </summary>
        /// <param name="dateTime">The DateTime object</param>
        /// <param name="includeTime">(Optional) If time should be included or not</param>
        /// <returns>DateTime instance from the specified native System.DateTime object</returns>
        public static DateTime CreateFromNativeDateTime(System.DateTime dateTime, bool includeTime = false) {
            DateTime retDateTime;
            string result = Strings.FormatInt(dateTime.Year, 4) + "-"
                + Strings.FormatInt(dateTime.Month, 2) + "-"
                + Strings.FormatInt(dateTime.Day, 2);
            if (includeTime) {
                result += " " + Strings.FormatInt(dateTime.Hour, 2) + ":" + Strings.FormatInt(dateTime.Minute, 2) + ":00";
            }
            retDateTime = new DateTime(result);
            return retDateTime;
        }


        /// <summary>
        /// Get the month name of a month's numeric representation
        /// </summary>
        /// <param name="month">The month's numeric representation</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static String GetMonthName(uint month)
        {
            if (month > 12)
                throw new ArgumentOutOfRangeException("month");
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)month);
        }


        /// <summary>
        /// Get the abbreviated month name of a month's numeric representation
        /// </summary>
        /// <param name="month">The month's numeric representation</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static String GetMonthNameShort(uint month)
        {
            if (month > 12)
                throw new ArgumentOutOfRangeException("month");
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName((int)month);
        }


        // <End::Static methods>
    }


    public class InvalidDateTimeException : Exception {
        public InvalidDateTimeException()
            : base("Value didn't comply to standard Date/Time format") { }
    }
}
