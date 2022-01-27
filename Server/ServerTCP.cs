using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Server
{
    class ServerTCP
    {
        private ClientCommands _clientCommands;
        private UsersDataBase _usersDataBase;
        private MessangerSender _messangerSender;
        private RaitingCommand _raitingCommand;

        public void StartServerTCP()
        {
            InitTypes();
            InitConnection();
        }

        public void SortTopPlayerInWorld(object obj)
        {
            Console.WriteLine("Отсартировалось");
            _raitingCommand.SortTopInWorld();
        }

        private void InitTypes()
        {
            Console.WriteLine("Инициализированно");
            _usersDataBase = new UsersDataBase();
            _raitingCommand = new RaitingCommand();
            _messangerSender = new MessangerSender();
            _clientCommands = new ClientCommands(_usersDataBase, _messangerSender);
            //TimerCallback tm = new TimerCallback(SortTopPlayerInWorld);
            //Timer timer = new Timer(tm, null, 0, 86400000);
        }

        private void InitConnection()
        {

            TcpListener ServerSocket = new TcpListener(IPAddress.Parse("127.0.0.2"), 11100);
            ServerSocket.Start();
            Reciver(ServerSocket);
        }

        public void Reciver(TcpListener ServerSocket)
        {
            while (true)
            {
                Console.WriteLine("Ждем!");
                TcpClient client = ServerSocket.AcceptTcpClient();
                Console.WriteLine("Пришел!");

                ClientProcess.StartProcessAsync(client, _clientCommands);
            }
            

            //while (true)
            //{
            //    NetworkStream stream = client.GetStream();
            //    byte[] buffer = new byte[1024];
            //    int byte_count = 0;
            //    if (stream.DataAvailable)
            //    {
            //        byte_count = stream.Read(buffer, 0, buffer.Length);
            //        string data = Encoding.UTF8.GetString(buffer, 0, byte_count);
            //        CommandAndMessages commandAndMessages = GetReciveCommandAndMessage(data);
            //        _clientCommands.UseCommand(commandAndMessages, client);
            //        Console.WriteLine(data);

            //        if (data == "ShoutDown")
            //        {
            //            Console.WriteLine("Сервер завершил соединение с клиентом. " + client);
            //            break;
            //        }
            //    }
            //}

            //client.Client.Shutdown(SocketShutdown.Both);
            //client.Close();
        }



    }


    public struct CommandAndMessages
    {
        public string Command { get; private set; }
        public string[] Messages { get; private set; }

        public CommandAndMessages(string command , string[] messages)
        {
            Command = command;
            Messages = messages;
        }
    }
}
