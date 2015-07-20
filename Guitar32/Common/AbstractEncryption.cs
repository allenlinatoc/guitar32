using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guitar32.Common
{
    public abstract class AbstractEncryption
    {

        private String _key;
        private byte[] _salt;
        private string _raw;
        private string _encrypted;


        protected String getKey() {
            return _key;
        }
        protected byte[] getSalt() {
            return _salt;
        }

        protected void setKey(string key) {
            _key = key;
        }
        protected void setSalt(byte[] salt) {
            this._salt = salt;
        }
        protected void setSalt(string salt) {
            _salt = Encoding.ASCII.GetBytes(salt);
        }
        protected void setValue(String value, bool isencrypted)
        {
            if (isencrypted)
            {
                this._encrypted = value;
                this._raw = null;
                return;
            }
            this._raw = value;
            this._encrypted = null;
        }


        //
        // User-defined methods
        //

        abstract public String Decrypt();
        abstract public String Encrypt();



    }
}
