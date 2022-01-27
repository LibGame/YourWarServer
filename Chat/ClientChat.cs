using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Clients;

namespace YourWarServer.Chat
{
    public struct ClientChat 
    {
        private string _login;
        private IPEndPoint _iPEndPoint;
        private string _username;

        public string Login { get => _login; }
        public IPEndPoint IPEndPoint { get => _iPEndPoint; }
        public string Username { get => _login; }

        public ClientChat(IPEndPoint iPEndPoint , string login, string ussername)
        {
            _username = ussername;
            _iPEndPoint = iPEndPoint;
            _login = login;
        }
    }
}
