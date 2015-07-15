using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Cryptography
{

    /// <summary>
    /// Signature object containing a Key and an Initialization Vector
    /// </summary>
    public class KeyIVSignature
    {

        private byte[] key;
        private byte[] iv;

        /// <summary>
        /// Create new instance of KeyIVSignature
        /// </summary>
        /// <param name="key">The 32-bit signature key</param>
        /// <param name="iv">The 16-bit signature initialization vector</param>
        public KeyIVSignature(byte[] key, byte[] iv)
        {
            this.key = key;
            this.iv = iv;
        }

        /// <summary>
        /// Get the signature key
        /// </summary>
        /// <returns></returns>
        public byte[] GetKey()
        {
            return key;
        }

        /// <summary>
        /// Get the signature initialization vector
        /// </summary>
        /// <returns></returns>
        public byte[] GetIV()
        {
            return iv;
        }

    }
}
