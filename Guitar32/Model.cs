using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guitar32.Database;

namespace Guitar32
{
    public abstract class Model
    {
        private int id;
        private SystemResponse response;

        public int getId() {
            return this.id;
        }

        public SystemResponse getResponse() {
            return this.response;
        }

        public Boolean exists() {
            return this.getId() >= 0;
        }

        protected void removeId() {
            this.id = -1;
        }

        protected void setId(int id) {
            this.id = id;
        }

        public void setResponse(SystemResponse response) {
            this.response = response;
        }
    }
}
