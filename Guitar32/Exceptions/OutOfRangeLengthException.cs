﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Exceptions
{
    public class OutOfRangeLengthException : Exception
    {
        public OutOfRangeLengthException() : base("String length is out of range")
        { }
    }
}
