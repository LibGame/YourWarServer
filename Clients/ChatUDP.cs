using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace YourWarServer.Clients
{
    public class ChatUDP
    {
        private bool alive = false; // будет ли работать поток для приема
        private UdpClient client;
        private const int LOCALPORT = 8001; // порт для приема сообщений
        private const int REMOTEPORT = 8001; // порт для отправки сообщений
        private const int TTL = 20;
        private const string HOST = "235.5.5.1"; // хост для групповой рассылки
        private IPAddress groupAddress; // адрес для групповой рассылки
        
        public void StartChat()
        {
            groupAddress = IPAddress.Parse(HOST);
            client = new UdpClient(LOCALPORT);
            client.JoinMulticastGroup(groupAddress, TTL);

            Task receiveTask = new Task(ReceiveMessages);
            receiveTask.Start();
            Console.WriteLine("Чат запущен");
        }

        private void ReceiveMessages()
        {
            alive = true;
            try
            {
                while (alive)
                {
                    IPEndPoint remoteIp = null;
                    byte[] data = client.Receive(ref remoteIp);
                    string message = Encoding.Unicode.GetString(data);

                    Console.WriteLine(message);
                }
            }
            catch (ObjectDisposedException)
            {
                if (!alive)
                    return;
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private void ExitChat()
        {
            client.DropMulticastGroup(groupAddress);

            alive = false;
            client.Close();

        }
 
    }
}

