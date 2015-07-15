using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Caching
{
    /// <summary>
    /// List/Collection of CacheItem data
    /// </summary>
    /// <typeparam name="TKey">Datatype of Key</typeparam>
    /// <typeparam name="TValue">Datatype of Value</typeparam>
    public class CacheItemCollection<TKey, TValue> : List<CacheItem<TKey, TValue>>
    {

        public CacheItemCollection() : base()
        {
            
        }

        public CacheItemCollection(IEnumerable<CacheItem<TKey, TValue>> cacheItemCollection)
            : base(cacheItemCollection)
        {

        }


        /// <summary>
        /// Check if this CacheItemCollection contains a key
        /// </summary>
        /// <param name="key">The CacheItem key to be looked up</param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            for (int i = 0; i < this.Count; i++)
            {
                CacheItem<TKey, TValue> cacheItem = this[i];

                // Check if key is equal to this one
                if (cacheItem.getKey().Equals(key))
                {

                    // Check if current item is expired
                    if (cacheItem.IsExpired())
                        cacheItem.Update();
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Get a cache item based on the specified CacheItem key
        /// </summary>
        /// <param name="key">The CacheItem key to be looked up</param>
        /// <returns></returns>
        public CacheItem<TKey, TValue> GetCacheItem(TKey key)
        {
            for (int i = 0; i < this.Count; i++)
            {
                CacheItem<TKey, TValue> cacheItem = this[i];

                // Check if key is equal to this one
                if (cacheItem.getKey().Equals(key))
                {
                    // Check if current item is expired
                    if (cacheItem.IsExpired())
                        cacheItem.Update();
                    return cacheItem;
                }
            }
            return null;
        }


        /// <summary>
        /// Get new CacheItemCollection instance that has the specified CacheItem value
        /// </summary>
        /// <param name="value">The CacheItem value to be looked up</param>
        /// <returns></returns>
        public CacheItemCollection<TKey, TValue> GetCacheItemsWithValue(TValue value)
        {
            CacheItemCollection<TKey, TValue> newCacheItems = new CacheItemCollection<TKey, TValue>();
            foreach (CacheItem<TKey, TValue> cacheItem in this)
            {
                if (cacheItem.getValue().Equals(value))
                {
                    cacheItem.Update();
                    newCacheItems.Add(cacheItem);
                }
            }

            return newCacheItems;
        }


        /// <summary>
        /// Validate the existence of the cache at a specified index. Returns boolean on whether the cache item is expired and has been updated; otherwise, false.
        /// </summary>
        /// <param name="index">The index of the Cache Item to be validated</param>
        /// <returns></returns>
        public bool ValidateExpiry(int index)
        {
            if (index < this.Count)
                throw new NullReferenceException();
            if (index >= this.Count)
                throw new ArgumentOutOfRangeException();

            if (this[index].IsExpired())
                this[index].Update();

            return false;
        }


        /// <summary>
        /// Validate the existence of the cache through the specified key. Returns boolean on whether the cache item is expired and has been removed, otherwise, false.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ValidateExistence(TKey key)
        {
            for (int i = 0; i < this.Count; i++)
            {
                CacheItem<TKey, TValue> cacheItem = this[i];
                if (cacheItem.getKey().Equals(key))
                {
                    return ValidateExpiry(i);
                }
            }
            return false;
        }


    }
}
