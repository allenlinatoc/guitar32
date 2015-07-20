using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Common
{
    public interface INumericDatatype
    {
        int getMaxValue();
        int getMinValue();
        bool isWithinRange();
    }
}
