using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guitar32.Database;

namespace Guitar32.Caching.Stores
{
    /// <summary>
    /// Cache object that can store single-row result (QueryResultRow objects)
    /// </summary>
    public class DatabaseRowCache : Cache<string, QueryResultRow>
    {

        /// <summary>
        /// Construct new instance of DatabaseRowCache
        /// </summary>
        public DatabaseRowCache()
            : base()
        {

        }

        public void BindQueryEvent(string query, int TTLSeconds = 300)
        {
            if (!this.ContainsKey(query))
                return;
            CacheItem<string, QueryResultRow> item = this.GetCacheItem(query);
            item.setTTL(TTLSeconds);
            item.OnCacheUpdate += new CacheItem<string, QueryResultRow>.cache_Expired(cache_Expired);
        }

        public void cache_Expired(CacheItem<string, QueryResultRow> sender, EventArgs e)
        {
            DatabaseCredentials creds = sender.getValue().GetSourceCredentials();
            using (DatabaseConnection conn = new DatabaseConnection(creds, true))
            {
                QueryBuilder query = new QueryBuilder(sender.getKey());
                sender.setValue(conn.QuerySingle(query));
            }
        }

    }
}