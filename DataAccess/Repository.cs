using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Repository
    {
        public string connectionString { get; set; }
        public string databaseName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }

        public bool ConnectToDatabase()
        {
            return true;
        }

    }
}
