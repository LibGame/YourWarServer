using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Server
{
    class IconCommand
    {
        private UsersDataBase _usersDataBase;


        public IconCommand(UsersDataBase usersDataBase)
        {
            _usersDataBase = usersDataBase;
        }



        public void ReciveBytesImage(string bytes)
        {

        }

    }
}
