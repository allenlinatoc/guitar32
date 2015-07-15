using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guitar32.Data;
using MySql.Data.MySqlClient;

namespace Guitar32.Database
{

    /// <summary>
    /// Lets you build database queries
    /// </summary>
    public class QueryBuilder
    {
        private String queryString;


        /// <summary>
        /// Instantiate an instance of QueryBuilder
        /// </summary>
        /// <param name="queryString">(Optional) Existing query string to be incorporated inside this query builder</param>
        public QueryBuilder(String queryString="")
        {
            this.queryString = queryString;
        }


        /// <summary>
        /// Delete an entry from table
        /// </summary>
        /// <param name="table">Target source table</param>
        /// <returns>Current instance</returns>
        public QueryBuilder DeleteFrom(String table) {
            this.queryString += "DELETE";
            this.padQuery();
            return this.From(table);
        }


        /// <summary>
        /// From what table/s shall the query be executed?
        /// </summary>
        /// <param name="table">Name/s of table. Separate by commas if more than one is specified</param>
        /// <returns>Current instance</returns>
        public QueryBuilder From(String table = "") {
            this.queryString += "FROM " + (table.Trim().Length > 0 ? table : "*");
            this.padQuery(true);
            return this;
        }
        /// <summary>
        /// From what table/s shall the query be executed?
        /// </summary>
        /// <param name="queryInstance">The queryBuilder instance where source of data will come from</param>
        /// <returns>Current instance</returns>
        public QueryBuilder From(QueryBuilder queryInstance)
        {
            this.queryString += "FROM (" + queryInstance.getQueryString() + ")";
            this.padQuery(true);
            return this;
        }


        /// <summary>
        /// GROUP BY expression
        /// </summary>
        /// <param name="columnName">The target name of column to be grouped by</param>
        /// <returns></returns>
        public QueryBuilder GroupBy(String columnName)
        {
            this.queryString += "GROUP BY " + columnName;
            this.padQuery(true);
            return this;
        }


        /// <summary>
        /// Insert expression
        /// </summary>
        /// <param name="tablename">The name of the target table</param>
        /// <param name="columns">(Optional) Array of column names</param>
        /// <returns>Current instance</returns>
        public QueryBuilder InsertInto(String tablename, String[] columns = null) {
            this.queryString = "INSERT INTO `" + tablename + "`" 
                + (columns!=null ? "("+String.Join(",", columns)+")" : "");
            this.padQuery(true);
            return this;
        }



        /// <summary>
        /// ORDER BY syntax
        /// </summary>
        /// <param name="columnName">The target column to be sorted</param>
        /// <param name="is_ascending">(Optional) If ordering will be ascending or descending</param>
        /// <returns></returns>
        public QueryBuilder OrderBy(string columnName, bool is_ascending = true)
        {
            this.queryString += "ORDER BY " + columnName;
            if (!is_ascending)
                this.queryString += " DESC";
            this.padQuery(true);
            return this;
        }



        /// <summary>
        /// Select a portion from table
        /// </summary>
        /// <param name="columns">Array of column names</param>
        /// <returns>Current instance</returns>
        public QueryBuilder Select(string[] columns = null)
        {
            if (columns != null) {
                for (int i = 0; i < columns.Length; i++) {
                    columns[i] = columns[i].Trim();
                }
                this.queryString = this.queryString = "SELECT " + String.Join(",", columns);
            }
            else {
                this.queryString = "SELECT *";
            }
            this.padQuery(true);
            return this;
        }
        /// <summary>
        /// Select a portion from table
        /// </summary>
        /// <param name="columns">Array of columns</param>
        /// <returns></returns>
        public QueryBuilder Select(params object[] columns)
        {
            List<string> columnNames = new List<string>();
            for (int i = 0; i < columns.Length; i++)
            {
                columnNames.Add(columns[i].ToString());
            }
            return this.Select(columnNames.Count == 0 ? null : columnNames.ToArray());
        }
        /// <summary>
        /// Select a column from table
        /// </summary>
        /// <param name="column">The column to be selected</param>
        /// <returns>Current instance</returns>
        public QueryBuilder Select(string column)
        {
            this.queryString = "SELECT " + column.Trim();
            this.padQuery(true);
            return this;
        }



        /// <summary>
        /// SET syntax to set column values in UPDATE operations
        /// </summary>
        /// <param name="setPairs">The pair of columns and its corresponding values</param>
        /// <returns>Current instance</returns>
        public QueryBuilder Set(Dictionary<string, string> setPairs) {
            List<string> lSetPairs = new List<string>();
            string strSetPairs;
            foreach (KeyValuePair<string, string> kv in setPairs) {
                lSetPairs.Add(kv.Key + " = " + kv.Value);
            }
            strSetPairs = String.Join(",", lSetPairs.ToArray());
            this.queryString += "SET " + strSetPairs;
            this.padQuery(true);
            return this;
        }
        /// <summary>
        /// SET syntax to set column values in UPDATE operations
        /// </summary>
        /// <param name="key">The key as column name</param>
        /// <param name="value">The value of the specified column</param>
        /// <returns>Current instance</returns>
        public QueryBuilder Set(string key, string value) {
            this.queryString += "SET " + key + " = " + value;
            this.padQuery(true);
            return this;
        }


        /// <summary>
        /// Update the table
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public QueryBuilder Update(String tablename) {
            this.queryString = "UPDATE " + tablename;
            this.padQuery(true);
            return this;
        }


        /// <summary>
        /// Prepared statement builder for INSERT
        /// </summary>
        /// <param name="tablename">Target table name</param>
        /// <param name="parameters">Target parameters</param>
        /// <returns>The array of MySqlParameter objects</returns>
        public MySqlParameter[] PreparedInsert(string tablename, MySqlParameter[] parameters)
        {
            // build column names
            List<string> paramNames = new List<string>();
            List<object> paramNamesVirtualValues = new List<object>();
            foreach (MySqlParameter param in parameters)
            {
                paramNames.Add(param.ParameterName);
                paramNamesVirtualValues.Add("@" + param.ParameterName);
            }

            this.InsertInto(tablename, paramNames.ToArray())
                .Values(paramNamesVirtualValues.ToArray());

            this.padQuery(true);

            return parameters;
        }

        /// <summary>
        /// Prepared statement builder for UPDATE
        /// </summary>
        /// <param name="tablename">Target table name</param>
        /// <param name="parameters">Target parameters</param>
        /// <param name="conditions">(Optional) Array of conditions</param>
        /// <returns>The array of MySqlParameter objects</returns>
        public MySqlParameter[] PreparedUpdate(string tablename, MySqlParameter[] parameters, KVCondition[] conditions = null)
        {
            this.queryString = "UPDATE " + tablename + Environment.NewLine;
            this.queryString += "SET ";
            List<string> listParams = new List<string>();
            foreach (MySqlParameter param in parameters)
            {
                listParams.Add(
                    string.Format("`{0}` = {1}"
                        , param.ParameterName.ToLower()
                        , "@" + param.ParameterName.ToLower()
                    )
                );
            }

            this.queryString += string.Join(Environment.NewLine, listParams.ToArray());
            this.padQuery(true);

            // Check for any condition/s specified
            if (conditions != null)
                this.Where(conditions);

            return parameters;
        }


        /// <summary>
        /// Insert an array of values
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>Current instance</returns>
        public QueryBuilder Values(object[] values) {
            this.queryString += "VALUES(" + String.Join(",", values) + ")";
            this.padQuery(true);
            return this;
        }



        /// <summary>
        /// Attach condition to your query
        /// </summary>
        /// <param name="conditions">Condition string to be attached</param>
        /// <returns>Current instance</returns>
        public QueryBuilder Where(String conditions)
        {
            this.queryString += "WHERE " + conditions;
            this.padQuery(true);
            return this;
        }
        /// <summary>
        /// Attach conditions to your query separated by logical operand "AND"
        /// </summary>
        /// <param name="conditions">Associative list of conditions</param>
        /// <param name="noTilde">(Optional) Tilde character for column names will be removed</param>
        /// <returns>Current instance</returns>
        public QueryBuilder Where(Dictionary<object, object> conditions, Boolean noTilde=false)
        {
            string conditionStr = "";
            foreach (KeyValuePair<object, object> condition in conditions) {
                conditionStr += (conditionStr.Length > 0 ? "AND `" : "`") + condition.Key + "`=" + condition.Value + " ";
            }
            if (conditionStr.Trim().Length == 0) {
                throw new ArgumentNullException(conditionStr);
            }
            if (noTilde) {
                conditionStr = conditionStr.Replace("`", "");
            }
            return this.Where(conditionStr.Trim());
        }
        /// <summary>
        /// Attach conditions to your query separated by optional condition link (logical operand)
        /// </summary>
        /// <param name="conditions">Set of conditions</param>
        /// <returns></returns>
        public QueryBuilder Where(params KVCondition[] conditions)
        {
            StringBuilder strbuildCondition = new StringBuilder();
            for (int i = 0; i < conditions.Length; i++)
            {
                KVCondition condition = conditions[i];
                if (i > 0)
                    strbuildCondition.Append(" ");
                strbuildCondition.Append(condition.GetCondition() + " " + (i < conditions.Length - 1 ? (condition.HasConditionLink() ? condition.ConditionLink : KVCondition.AND) : ""));
            }
            return this.Where(strbuildCondition.ToString().Trim());
        }
        /// <summary>
        /// Attach single column-value-based condition to your query
        /// </summary>
        /// <param name="column">The column name</param>
        /// <param name="value">The expected column value</param>
        /// <param name="noTilde">(Optional) Tilde character for column names will be removed</param>
        /// <returns>Current instance</returns>
        public QueryBuilder Where(String column, object value, Boolean noTilde=false) {
            Dictionary<object, object> condition = new Dictionary<object, object>();
            condition.Add(column, value);
            return this.Where(condition, noTilde);
        }



        // <Begin::Getters and Setters>

        /// <summary>
        /// Get current query string
        /// </summary>
        /// <returns></returns>
        public String getQueryString() {
            return this.queryString;
        }

        // <End::Getters and Setters>



        private void padQuery(bool isNewLine = false)
        {
            this.queryString = Utilities.Strings.RightTrim(this.queryString)
                + (isNewLine ? "\n" : " ");
        }
    }
}
