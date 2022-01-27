using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace YourWarServer.Data
{
    public abstract class DataBase
    {
        public abstract SqlConnection SqlConnection { get; }

        public abstract void CreateDataBase();
    }
}
