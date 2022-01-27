using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace YourWarServer.Clients
{
    public abstract class Client
    {
        protected int _id;
        protected NetworkStream _stream;
        protected string _userName;
        protected TcpClient _client;

        public abstract int ID { get; }
        public abstract NetworkStream Stream { get; }
        public abstract string UserName { get; }
        public abstract TcpClient TcpClient { get; }
    }
}
