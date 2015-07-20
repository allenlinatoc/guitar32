using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guitar32.Database;

namespace Guitar32.Database
{
    [Serializable]
    public class DatabaseCredentialsList : ListState<DatabaseCredentials>
    {

        private string sourceFile = null;

        public DatabaseCredentialsList()
            : base()
        { }

        public DatabaseCredentialsList(IEnumerable<DatabaseCredentials> list)
            : base(list)
        { }


        public new void Add(DatabaseCredentials item)
        {
            foreach (DatabaseCredentials cred in this)
            {
                if (cred.getServer() == item.getServer() &&
                    cred.getPort() == item.getPort() &&
                    cred.getUsername() == item.getUsername() &&
                    cred.getPassword() == item.getPassword() &&
                    cred.getDatabase() == item.getDatabase())
                    return;
            }
            base.Add(item);
            
            // Update source file, if there is
            if (this.sourceFile != null)
                this.Save(sourceFile);
        }

        public new void Remove(DatabaseCredentials creds)
        {
            this.Remove(creds.getServer(), creds.getPort(), creds.getUsername(), creds.getPassword(), creds.getDatabase());
        }

        public void Remove(string server, uint port, string username, string password, string database)
        {
            for (int i = 0; i < this.Count; i++)
            {
                DatabaseCredentials cred = this[i];
                if (cred.getServer() == server &&
                    cred.getPort() == port &&
                    cred.getUsername() == username &&
                    cred.getPassword() == password &&
                    cred.getDatabase() == database)
                {
                    base.RemoveAt(i);
                    if (this.sourceFile != null)
                        this.Save(sourceFile);
                }
            }
        }

        public new void Save(string path)
        {
            Utilities.ObjectSerializer.SerializeToFile(path, this);
        }

        /// <summary>
        /// Set the file where this DBL (Database Credentials List) data came from
        /// </summary>
        /// <param name="path">The path to the source DBL file</param>
        public void setSourceFile(string path)
        {
            this.sourceFile = path;
        }

        /// <summary>
        /// Get the path file where this DBL (Database Credentials List) data came from
        /// </summary>
        /// <returns></returns>
        public string getSourceFile()
        {
            return this.sourceFile;
        }

        static new public DatabaseCredentialsList CreateFromFile(string path)
        {
            DatabaseCredentialsList dbCredsList;

            //try
            //{
                dbCredsList =
                    ((DatabaseCredentialsList)Utilities.ObjectSerializer.DeserializeFromFile<DatabaseCredentialsList>(path));
                dbCredsList.setSourceFile(path);
                Console.WriteLine("Success reading!");
            //}
            //catch
            //{
            //    dbCredsList = new DatabaseCredentialsList();
            //}
            return dbCredsList;
        }

    }
}
