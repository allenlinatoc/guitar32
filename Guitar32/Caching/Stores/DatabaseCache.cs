using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guitar32.Database;

namespace Guitar32.Caching.Stores
{
    /// <summary>
    /// Cache object that can store multi-row results (QueryResult objects)
    /// </summary>
    public class DatabaseCache : Cache<string, QueryResult>
    {

        /// <summary>
        /// Construct new instance of DatabaseCache
        /// </summary>
        public DatabaseCache()
            : base()
        {

        }


    }
}
