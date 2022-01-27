using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YourWarServer.Clients;
using YourWarServer.Server;
using YourWarServer.Data;
using YourWarServer.Chat;

namespace YourWarServer
{
    class Program
    {
        const int port = 8888;
        static TcpListener listener;
        private ChatClients _chatClients;

        static TcpListener tcpListener; // сервер для прослушивания

        static ServerTCP server; // сервер
        static Thread listenThread; // потока для прослушивания


        static void Main(string[] args)
        {
            DataBasePhasade dataBasePhasade = new DataBasePhasade();
            dataBasePhasade.CreateConnections();

  

            try
            {
                server = new ServerTCP();
                listenThread = new Thread(new ThreadStart(server.StartServerTCP));
                listenThread.Start();

                var chat = new ChatServer();
                chat.EnterToChat();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
