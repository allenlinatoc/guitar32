using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Database
{
    /// <summary>
    /// Class for storing a row from a QueryResult object
    /// </summary>
    public class QueryResultRow : Dictionary<string, object>
    {

        private DatabaseCredentials sourceCredentials;


        public QueryResultRow(DatabaseCredentials sourceCredentials) : base()
        {
            this.sourceCredentials = sourceCredentials;
        }

        public QueryResultRow(IDictionary<string, object> dictionary, DatabaseCredentials sourceCredentials) : base(dictionary)
        {
            this.sourceCredentials = sourceCredentials;
        }

        /// <summary>
        /// Get the source DatabaseCredentials object
        /// </summary>
        /// <returns></returns>
        public DatabaseCredentials GetSourceCredentials()
        {
            return this.sourceCredentials;
        }

    }
}
