using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// MySQL
using MySql.Data;
using MySql.Data.Common;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

using Guitar32.Caching;
using Guitar32.Caching.Stores;
using Guitar32.Exceptions;
using Guitar32.Utilities;



namespace Guitar32.Database
{
    public class DatabaseConnection : IDisposable
    {
        // defaults
        const string DEFAULT_CHARSET = "utf8";
        const DBMSTypes DEFAULT_TYPE = DBMSTypes.MySQL;


        protected DatabaseCredentials
            dbCredentials;
        protected DBMSTypes
            type;
        protected String
            characterSet;

        // Connection instances
        protected MySqlConnection
            mysql;

        private bool is_disposed;


        /// <summary>
        /// Create an instance of DatabaseConnection
        /// </summary>
        /// <param name="credentials">The credentials object to be used to connect to server</param>
        /// <param name="characterSet">(Optional) The character set to be used in the whole session</param>
        /// <param name="type">(Optional) The DBMS server type (e.g. Oracle, MySQL, etc.), refer to <code>DBMSTypes</code> enum</param>
        public DatabaseConnection(DatabaseCredentials credentials, String characterSet = DEFAULT_CHARSET, DBMSTypes type = DEFAULT_TYPE) {
            this.characterSet = characterSet;
            this.type = type;
            this.dbCredentials = credentials;
            this.is_disposed = false;
        }


        /// <summary>
        /// Create an instance of DatabaseConnection with auto-connect option
        /// </summary>
        /// <param name="credentials">The credentials object to be used to connect to server</param>
        /// <param name="connect">Boolean value if this will connect to database after instantiation</param>
        public DatabaseConnection(DatabaseCredentials credentials, bool connect)
        {
            this.characterSet = DEFAULT_CHARSET;
            this.type = DEFAULT_TYPE;
            this.dbCredentials = credentials;
            this.is_disposed = false;
            if (connect)
                Connect();
        }

        ~DatabaseConnection()
        {
            if (this != null)
                Dispose();
        }



        /// <summary>
        /// Start connecting to database
        /// </summary>
        /// <returns>If connection to database is success</returns>
        public bool Connect() {
            switch(this.type) {
                case DBMSTypes.MySQL: {
                    if (!this.IsConnected()) {
                        DatabaseCredentials creds = this.dbCredentials;
                        MySqlConnectionStringBuilder connStr = new MySqlConnectionStringBuilder();
                        connStr.Server = creds.getServer();
                        connStr.Port = creds.getPort();
                        connStr.UserID = creds.getUsername();
                        connStr.Password = creds.getPassword();
                        connStr.Database = creds.getDatabase();
                        connStr.CharacterSet = "utf8";
                        this.mysql = new MySqlConnection(connStr.GetConnectionString(true));
                        try {
                            this.mysql.Open();
                        }
                        catch (MySqlException ex) {
                            this.LogError(ex.Message);
                            Console.Write("Unable to connect to database: " + ex.Message);
                            return false;
                        }
                        return true;
                    }
                    else {
                        return true;
                    }
                    // END MySQL
                    }

                default: {
                    return false;
                    }
            }
        }


        /// <summary>
        /// Execute a non-scalar SQL query
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <param name="parameters">An array of MySqlParameter objects.</param>
        /// <returns>If execution of the query is success</returns>
        public bool Execute(string query, MySqlParameter[] parameters = null) {
            if (!this.IsConnected()) {
                DisconnectedException exception = new DisconnectedException();
                this.LogError(exception.Message);
                throw exception;
            }
            MySqlCommand cmd = new MySqlCommand(query, this.mysql);
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            cmd.EnableCaching = true;
            try {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex) {
                this.LogError(ex.Message, ex.ErrorCode);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Execute a non-scalar SQL query
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <param name="parameters">(Optional) Parameters to be passed</param>
        /// <returns>If execution of the query is success</returns>
        public bool Execute(QueryBuilder query, MySqlParameter[] parameters = null)
        {
            return this.Execute(query.getQueryString(), parameters);
        }




        /// <summary>
        /// Check if a query returns a row
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <returns>If a query returns a row</returns>
        public bool Exists(string query) {
            if (!this.IsConnected()) {
                DisconnectedException exception = new DisconnectedException();
                this.LogError(exception.Message);
                throw exception;
            }

            Dictionary<string,object> result = this.QuerySingle(query);
            if (result != null) {
                return result.Count > 0;
            }
            return false;
        }
        /// <summary>
        /// Check if a query returns a row
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <returns>If a query returns a row</returns>
        public bool Exists(QueryBuilder query) {
            return this.Exists(query.getQueryString());
        }



        /// <summary>
        /// Get the LAST_INSERT_ID from this current MySQL exception, returns -1 on failure/null
        /// </summary>
        /// <returns>The LAST_INSERT_ID from this current MySQL exception, otherwise, -1 on failure/null</returns>
        public int GetLastInsertID() {
            if (this.IsConnected()) {
                QueryBuilder query = new QueryBuilder();
                query.Select("LAST_INSERT_ID() as id");
                Dictionary<string, object> row = this.QuerySingle(query);
                if (row != null && row.Count > 0) {
                    return int.Parse(row["id"].ToString());
                }
                return -1;
            }
            else {
                DisconnectedException exception = new DisconnectedException();
                this.LogError(exception.Message);
                throw exception;
            }
        }




        /// <summary>
        /// Check if this Database connection instance is currently connected or not
        /// </summary>
        /// <returns>If this Database connection instance is currently connected or not</returns>
        public bool IsConnected() {
            switch (this.type) {

                // MySQL
                case DBMSTypes.MySQL: {
                    if (this.mysql == null) {
                        return false;
                    }
                    return this.mysql.State == System.Data.ConnectionState.Open
                        || this.mysql.State == System.Data.ConnectionState.Fetching
                        || this.mysql.State == System.Data.ConnectionState.Executing
                        || this.mysql.State == System.Data.ConnectionState.Connecting;
                    } // END of MySQL

                // Oracle
                case DBMSTypes.Oracle: {

                    return false;
                    } // END of Oracle

                // PostgreSQL
                case DBMSTypes.PostgreSQL: {

                    return false;
                    } // END of PostgreSQL

                default: {
                    return false;
                    }
            }
        }


        /// <summary>
        /// Execute query expecting multi-row result
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <param name="parameters">(Optional) Array of MySqlParameter objects to be passed</param>
        /// <param name="cache">(Optional) Cache where possible data existence will be verified</param>
        /// <param name="forceUpdate">(Optional) If cache shall be forced to be updated</param>
        /// <returns>The resulting QueryResult object</returns>
        public QueryResult Query(string query, MySqlParameter[] parameters = null, DatabaseCache cache = null, bool forceUpdate = false)
        {

            // Check for cache
            if (cache != null && !forceUpdate)
                if (cache.ContainsKey(query))
                {
                    CacheItem<string, QueryResult> cacheItem = cache.GetCacheItem(query);
                    if (cacheItem.IsExpired())
                    {
                        // Cache is expired, `forceUpdate` activated
                        forceUpdate = true;
                    }
                    else
                    {
                        Console.WriteLine("Update from cache!");
                        return cacheItem.getValue();
                    }
                }

            Console.WriteLine("Not from cache!");

            if (!this.IsConnected())
            {
                DisconnectedException exception = new DisconnectedException();
                this.LogError(exception.Message);
                throw exception;
            }

            MySqlCommand cmd = new MySqlCommand(query, this.mysql);

            // Check for MySqlParameters
            if (parameters != null)
            {
                foreach (MySqlParameter param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
            }

            MySqlDataReader reader = null;
            try {
                reader = cmd.ExecuteReader();
            }
            catch (MySqlException ex) {
                this.LogError(ex.Message, ex.ErrorCode);
                throw ex;
            }

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            while (reader.Read()) {
                Dictionary<string, object> row = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++) {
                    if (reader.IsDBNull(i)) {
                        row.Add(reader.GetName(i), null);
                    }
                    else {
                        String columnName = reader.GetName(i);
                        object value;

                        if (reader.GetFieldType(i).Equals(typeof(MySql.Data.Types.MySqlDateTime)))
                        {
                            value = reader.GetDateTime(i);
                        }
                        else if (reader.GetFieldType(i).ToString().ToUpper().Equals("SYSTEM.BYTE[]"))
                        {
                            long filesize = reader.GetBytes(i, 0, null, 0, 0);
                            //Console.WriteLine(filesize);
                            byte[] buffer = new byte[filesize];
                            reader.GetBytes(i, 0, buffer, 0, (int)filesize);
                            //File.WriteAllBytes(@"C:\b.wav", buffer);
                            value = buffer;
                            //MemoryStream ms = new MemoryStream(buffer);
                            //StreamReader sReader = new StreamReader(ms);
                            //value = sReader.ReadToEnd();
                        }
                        else
                        {
                            value = reader.GetString(i);
                        }
                        row.Add(columnName, value);
                    }
                }

                result.Add(row);
            }

            // Try to close the reader
            if (!reader.IsClosed) {
                reader.Close();
            }
            QueryResult queryResult = new QueryResult(result.ToArray(), this.dbCredentials);


            // Check this result can be cached
            if (cache != null || forceUpdate)
            {
                cache.Add(new CacheItem<string, QueryResult>(query, queryResult));
            }

            return queryResult;
        }



        /// <summary>
        /// Execute query expecting multi-row result
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <param name="parameters">(Optional) Array of MySqlParameter objects to be passed</param>
        /// <param name="cache">(Optional) Cache where possible data existence will be verified</param>
        /// <returns>The resulting QueryResult object</returns>
        public QueryResult Query(QueryBuilder query, MySqlParameter[] parameters = null, DatabaseCache cache = null) {
            return this.Query(query.getQueryString(), parameters, cache);
        }



        /// <summary>
        /// Execute query expecting single-row result, otherwise, null
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <param name="parameters">(Optional) Array of MySqlParameter objects to be passed</param>
        /// <param name="cache">(Optional) Cache where possible data existence will be verified</param>
        /// <returns>The resulting row of result, an array of columnar field values, otherwise, null</returns>
        public QueryResultRow QuerySingle(string query, MySqlParameter[] parameters = null, DatabaseRowCache cache = null, bool forceUpdate = false)
        {
            if (!this.IsConnected()) {
                DisconnectedException exception = new DisconnectedException();
                this.LogError(exception.Message);
                throw exception;
            }

            // Check for cache
            if (cache != null && !forceUpdate)
                if (cache.ContainsKey(query))
                {
                    CacheItem<string, QueryResultRow> cacheItem = cache.GetCacheItem(query);
                    if (cacheItem.IsExpired())
                    {
                        // Cache is expired, `forceUpdate` activated
                        forceUpdate = true;
                    }
                    else
                    {
                        Console.WriteLine("Update from cache!");
                        return cacheItem.getValue();
                    }
                }

            MySqlCommand cmd = new MySqlCommand(query, this.mysql);
            
            // Check for MySqlParameters
            if (parameters != null)
            {
                foreach (MySqlParameter param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
            }
            
            MySqlDataReader reader = null;
            try {
                reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);
            }
            catch (MySqlException ex) {
                this.LogError(ex.Message, ex.ErrorCode);
                throw ex;
            }

            QueryResultRow result = new QueryResultRow(this.dbCredentials);
            // Check if result returned a row
            if (!reader.HasRows) {
                if (!reader.IsClosed) {
                    reader.Close();
                }
                return null;
            }
            // Continue retrieving result
            if (reader.Read()) {
                for (int i=0; i<reader.FieldCount; i++) {
                    if (reader.IsDBNull(i)) {
                        result.Add(reader.GetName(i), null);
                    }
                    else {
                        String columnName = reader.GetName(i);
                        object value;

                        if (reader.GetFieldType(i).Equals(typeof(MySql.Data.Types.MySqlDateTime)))
                        {
                            value = reader.GetDateTime(i);
                        }
                        else if (reader.GetFieldType(i).ToString().ToUpper().Equals("SYSTEM.BYTE[]"))
                        {
                            long filesize = reader.GetBytes(i, 0, null, 0, 0);
                            //Console.WriteLine(filesize);
                            byte[] buffer = new byte[filesize];
                            reader.GetBytes(i, 0, buffer, 0, (int)filesize);
                            //File.WriteAllBytes(@"C:\b.wav", buffer);
                            value = buffer;
                            //MemoryStream ms = new MemoryStream(buffer);
                            //StreamReader sReader = new StreamReader(ms);
                            //value = sReader.ReadToEnd();
                        }
                        else
                        {
                            value = reader.GetString(i);
                        }
                        result.Add(columnName, value);
                    }
                }
            }
            // Try to close the reader
            if (!reader.IsClosed) {
                reader.Close();
            }

            // Check this result can be cached
            if (cache != null || forceUpdate)
            {
                cache.Add(new CacheItem<string, QueryResultRow>(query, result));
            }
            
            return result;
        }

        /// <summary>
        /// Execute query expecting single-row result, otherwise, null
        /// </summary>
        /// <param name="query">The query to be executed</param>
        /// <param name="parameters">(Optional) Array of MySqlParameter objects to be passed</param>
        /// <returns>The resulting row of result, an array of columnar field values, otherwise, null</returns>
        public QueryResultRow QuerySingle(QueryBuilder query, MySqlParameter[] parameters = null)
        {
            return this.QuerySingle(query.getQueryString(), parameters);
        }



        // <Begin::Getters and Setters>
        public string[][] GetLastResult() {
            return null;
        }
        // <End::Getters and Setters>




        // <Begin::Private>
        private void LogError(String message, int errorCode=-1) {
            Guitar32.Utilities.Diagnostics diagLog = new Guitar32.Utilities.Diagnostics();
            diagLog.LogEntry(message + (errorCode > -1 ? " (error code: "+errorCode+")" : "")
                , EventLogEntryType.Error);
        }
        // <End::Private>



        /// <summary>
        /// Dispose this connection from database
        /// </summary>
        public void Dispose()
        {
            if (this.mysql != null)
            {
                this.mysql.Close();
                this.mysql.Dispose();
            }
            this.is_disposed = true;
        }
    }



    /// <summary>
    /// List of common DBMS Types
    /// </summary>
    public enum DBMSTypes
    {
        Oracle, MySQL, MicrosoftSQL, PostgreSQL
    }
    
}
