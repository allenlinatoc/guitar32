using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Database
{
    /// <summary>
    /// Class for storing result from a database query
    /// </summary>
    public class QueryResult : IEnumerable<QueryResultRow>
    {
        private Dictionary<string, object>[] data;
        private DatabaseCredentials sourceCredentials;

        /// <summary>
        /// Instantiate an instance of QueryResult
        /// </summary>
        /// <param name="data">The database result to be passed</param>
        /// <param name="sourceCredentials">The source DatabaseCredentials object</param>
        public QueryResult(Dictionary<string, object>[] data, DatabaseCredentials sourceCredentials)
        {
            this.data = data;
            this.sourceCredentials = sourceCredentials;
        }


        /// <summary>
        /// Instantiate an instance of QueryResult
        /// </summary>
        /// <param name="data">The database row to be passed</param>
        /// <param name="sourceCredentials">The source DatabaseCredentials object</param>
        public QueryResult(Dictionary<string, object> data, DatabaseCredentials sourceCredentials)
        {
            List<Dictionary<string, object>> newList = new List<Dictionary<string, object>>();
            newList.Add(data);
            this.data = newList.ToArray();
            this.sourceCredentials = sourceCredentials;
        }


        /// <summary>
        /// Check if these rows contain a column name
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <returns>If these rows contain a column name</returns>
        public Boolean ContainsColumn(String columnName) {
            if (this.IsEmpty()) {
                return false;
            }
            Dictionary<string, object> samplerow = this.GetSingle(0);
            return samplerow.Keys.Contains(columnName);
        }


        /// <summary>
        /// Get a single row from the result
        /// </summary>
        /// <param name="index">The index of row from the result</param>
        /// <returns>The row from the result</returns>
        public QueryResultRow GetSingle(int index = -9999) {
            if (this.data.Length == 0) {
                throw new EmptyQueryResultException();
            }
            if (index == -9999) {
                return new QueryResultRow(this.data[0], this.GetSourceCredentials());
            }
            else {
                return new QueryResultRow(this.data[index], this.GetSourceCredentials());
            }
        }

        /// <summary>
        /// Get the source DatabaseCredentials
        /// </summary>
        /// <returns></returns>
        public DatabaseCredentials GetSourceCredentials()
        {
            return this.sourceCredentials;
        }

        /// <summary>
        /// Get the sub result from this result 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        public QueryResult SubResult(string columnName, object expectedValue)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            foreach (QueryResultRow row in this)
            {
                // Check if columnName exists
                if (!row.ContainsKey(columnName))
                    continue;

                // Check for equal value
                if (row["columnName"] != expectedValue)
                    continue;

                Dictionary<string, object> newrow = (Dictionary<string, object>)row;
                result.Add(newrow);
            }

            return new QueryResult(result.ToArray(), this.GetSourceCredentials());
        }

        /// <summary>
        /// Get the sub result from this result
        /// </summary>
        /// <param name="conditions">Set of conditions to be checked to extract result from this query</param>
        /// <returns></returns>
        public QueryResult SubResult(params KVCondition[] conditions)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            foreach (QueryResultRow row in this)
            {
                bool conditionsMet = false;

                // Check each condition
                foreach (KVCondition condition in conditions)
                {
                    if (!row.ContainsKey(condition.Key))
                        break;
                    if (row[condition.Key] != condition.Value)
                        break;

                    conditionsMet = true;
                }

                if (conditionsMet)
                    result.Add(row);
            }

            return new QueryResult(result.ToArray(), this.GetSourceCredentials());
        }


        /// <summary>
        /// Check if result is empty
        /// </summary>
        /// <returns></returns>
        public Boolean IsEmpty() {
            return this.data == null || this.data.Length==0;
        }


        /// <summary>
        /// Get the row count from this result
        /// </summary>
        /// <returns>The number of count from the result</returns>
        public int RowCount() {
            return this.data.Length;
        }


        /// <summary>
        /// Get an array of values from a column per row of this instance
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <returns>The array of values from a column per row of this instance</returns>
        public object[] ToArray(string columnName) {
            List<object> result = new List<object>();
            if (this.ContainsColumn(columnName)) {
                for (int i = 0; i < this.RowCount(); i++) {
                    Dictionary<string, object> row = this.GetSingle(i);
                    result.Add(row[columnName]);
                }
            }
            else {
                throw new NoColumnNameException();
            }
            return result.ToArray();
        }


        /// <summary>
        /// Get the Enumerator object of this instance
        /// </summary>
        /// <returns></returns>
        public IEnumerator<QueryResultRow> GetEnumerator()
        {
            List<QueryResultRow> ret = new List<QueryResultRow>();
            for (int i = 0; i < this.RowCount(); i++)
                ret.Add(GetSingle(i));
            return ret.GetEnumerator();
        }

        /// <summary>
        /// Inherited method to return the Enumerator objectS
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    /// <summary>
    /// Exception throw if tried to get a row from an empty QueryResult
    /// </summary>
    public class EmptyQueryResultException : Exception
    {
        public EmptyQueryResultException()
            : base("Trying to get a row from an empty QueryResult instance") { }
    }


    public class NoColumnNameException : Exception
    {
        public NoColumnNameException()
            : base("Trying to get a column name that does not exist") { }
    }
}
