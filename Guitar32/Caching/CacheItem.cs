using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Caching
{

    /// <summary>
    /// Object that can store cache data
    /// </summary>
    /// <typeparam name="TKey">The type of CacheItem key</typeparam>
    /// <typeparam name="TValue">The type of CacheItem value</typeparam>
    public class CacheItem<TKey, TValue>
    {
        /// <summary>
        /// Minimum Time-to-Live allowed in seconds
        /// </summary>
        public const int MIN_TTLSECONDS = 60;

        private TKey key;
        private TValue value;
        private int TTLSeconds;
        protected System.DateTime created;
        protected System.Timers.Timer timer;
        // Event
        public event cache_Expired OnCacheUpdate = null;
        public delegate void cache_Expired(CacheItem<TKey, TValue> sender, EventArgs e);
        


        /// <summary>
        /// Construct new instance of CacheItem
        /// </summary>
        /// <param name="TTLSeconds">Time-to-Live of this CacheItem in seconds</param>
        public CacheItem(int TTLSeconds = 300)
        {
            this.created = System.DateTime.Now;

            if (TTLSeconds < MIN_TTLSECONDS)
                throw new InvalidTTLSecondsException();

            this.TTLSeconds = TTLSeconds;

            // Initialize timer
            this.ResetTimer();
        }
        
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Update();
        }

        /// <summary>
        /// Construct new instance of CacheItem
        /// </summary>
        /// <param name="key">The key of this CacheItem</param>
        /// <param name="value">The value of this CacheItem</param>
        /// <param name="TTLSeconds">Time-to-Live of this CacheItem in seconds</param>
        public CacheItem(TKey key, TValue value, int TTLSeconds = 300)
        {
            this.key = key;
            this.value = value;
            this.created = System.DateTime.Now;

            if (TTLSeconds < MIN_TTLSECONDS)
                throw new InvalidTTLSecondsException();

            this.TTLSeconds = TTLSeconds;
        }

        /// <summary>
        /// Check if this CacheItem /mis still alive
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            System.DateTime now = System.DateTime.Now;
            return now.Subtract(this.getCreated()).Seconds > 0;
        }

        /// <summary>
        /// Get the System.DateTime when this was created
        /// </summary>
        /// <returns></returns>
        public System.DateTime getCreated()
        {
            return this.created;
        }

        /// <summary>
        /// Get the key of this CacheItem
        /// </summary>
        /// <returns></returns>
        public TKey getKey()
        {
            return this.key;
        }

        /// <summary>
        /// Get the TTL (Time-to-Live) of this CacheItem in seconds
        /// </summary>
        /// <returns></returns>
        public int getTTLSeconds()
        {
            return this.TTLSeconds;
        }

        /// <summary>
        /// Get the value of this CacheItem
        /// </summary>
        /// <returns></returns>
        public TValue getValue()
        {
            return this.value;
        }


        /// <summary>
        /// Check if this CacheItem is beyond the given TTL in seconds. Opposite of method IsAlive().
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return !IsAlive();
        }

        /// <summary>
        /// Set the key of this CacheItem
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private CacheItem<TKey, TValue> setKey(TKey key)
        {
            this.key = key;
            return this;
        }

        public CacheItem<TKey, TValue> setTTL(int TTLSeconds)
        {
            if (TTLSeconds < MIN_TTLSECONDS)
                throw new InvalidTTLSecondsException();
            this.TTLSeconds = TTLSeconds;
            return this;
        }

        /// <summary>
        /// Set the value of this CacheItem
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CacheItem<TKey, TValue> setValue(TValue value)
        {
            this.value = value;
            return this;
        }

        /// <summary>
        /// Reset the timer
        /// </summary>
        protected void ResetTimer()
        {
            this.timer = new System.Timers.Timer(TTLSeconds * 1000);
            this.timer.Elapsed += timer_Elapsed;
            this.timer.AutoReset = true;
            this.timer.Start();
        }

        /// <summary>
        /// Update this cache item. Returns boolean value whether this cache item has been successfully updated or not.
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            if (this.OnCacheUpdate != null && this.IsExpired())
            {
                this.OnCacheUpdate(this, null);
                this.created = System.DateTime.Now;

                // Reset timer
                this.ResetTimer();
                return true;
            }
            return false;
        }

    }


    /// <summary>
    /// Exception thrown when the number of TTL in seconds is below the minimum (60 seconds)
    /// </summary>
    public class InvalidTTLSecondsException : Exception
    {
        /// <summary>
        /// Construct new instance of InvalidTTLSecondsException
        /// </summary>
        public InvalidTTLSecondsException()
            : base("Invalid number of TTLSeconds was provided. Minimum of 60 seconds")
        { }

    }

}
