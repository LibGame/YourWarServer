using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Server
{
    public class MessageSenderUser
    {
        private UsersDataBase _usersDataBase;

        public MessageSenderUser(UsersDataBase usersDataBase)
        {
            _usersDataBase = usersDataBase;
        }

        public void SendPrivateMessageTo(string ussernameFrom ,string ussernameTo,string message)
        {
            string sendMessage = $"{ussernameFrom}/{message}/{DateTime.Now}/";
            string previous = _usersDataBase.GetUsserDataByLogin(ussernameTo, "ChatMessages");
            _usersDataBase.UpdateDataByLogin(ussernameTo, "ChatMessages", previous + sendMessage);
        }
    }
}