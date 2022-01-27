using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Data
{
    public class DataBasePhasade
    {
        private UsersDataBase _usersDataBase; 

        public void CreateConnections()
        {
            _usersDataBase = new UsersDataBase();
            _usersDataBase.CreateDataBase();
        }
    }
}
