using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Common
{
    public interface IDatabaseEntity
    {
        Boolean CreateData(); // create entry in database
        Boolean Delete(); // delete entry in database
        Boolean Update(); // update entry in database
    }
}
