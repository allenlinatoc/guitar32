using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Common
{
    public interface IStringDatatype
    {
        int getMaxLength();
        int getMinLength();
        bool isWithinRange();
    }
}
