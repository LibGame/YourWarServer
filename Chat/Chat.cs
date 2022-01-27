using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace YourWarServer.Chat
{
    public class Chat
    {
        protected volatile List<ClientChat> _clients = new List<ClientChat>(); 
        protected Queue<string> _messages = new Queue<string>();

        public void PrepareMessage(string login, IPEndPoint remoteFullIp, string message, string username)
        {
            Console.WriteLine(message);
            if (!CheckIfUsserInClinetsList(login))
            {
                
                Console.WriteLine("Добавили " + remoteFullIp.Address.ToString());
                _clients.Add(new ClientChat(remoteFullIp, login, username));
                SendLastMessages(remoteFullIp);
                Console.WriteLine("Отправили последние сообщения");
                //BroadcastMessage(message);
            }
            else
            {
                AddToQueue(message);
                BroadcastMessage(message);
                Console.WriteLine("Сообщения");
            }

        }

        public async void PrepareMessageAsync(string login, IPEndPoint remoteFullIp, string message, string username)
        {
            await Task.Run(() => PrepareMessage(login, remoteFullIp, message, username));
        }


        public void SendLastMessages(IPEndPoint remoteFullIp)
        {
            string result = "";

            int count = _messages.Count;
            if(count == 0)
            {
                result = "n";
            }
            else
            {
                int i = 0;
                foreach(var message in _messages)
                {
                    if (i + 1 < count)
                        result += message + "|";
                    else
                        result += message;
                    i++;
                }
            }

            Send(result, remoteFullIp);
        }

        private void AddToQueue(string message)
        {
            if(_messages.Count > 10)
            {
                _messages.Dequeue();
                _messages.Enqueue(message);
            }
            else
            {
                _messages.Enqueue(message);
            }
        }

        public void Remove(string login, IPEndPoint remoteFullIp)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].Login == login)
                {
                    Console.WriteLine("Удален");
                    _clients.RemoveAt(i);
                }
            }

            Send("Exited", remoteFullIp);
        }

        public async void RemoveAsync(string login, IPEndPoint remoteFullIp)
        {
            await Task.Run(() => Remove(login , remoteFullIp));
        }

        public bool CheckIfUsserInClinetsList(string ussername)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].Login == ussername)
                {
                    return true;
                }

            }
            return false;
        }

        public void BroadcastMessage(string message)
        {
            for (int i = 0; i < _clients.Count; i++) // Циклом перебераем всех клиентов
            {
                Send(message, _clients[i].IPEndPoint); // Отправляем сообщение

                //var clint = new UdpClient(_clients[i].IPEndPoint);             
                //if (IsConnected(new UdpClient(_clients[i].IPEndPoint).Client))
                //{
                //    Console.WriteLine("Отправилось");
                //    Send(message, _clients[i].IPEndPoint); // Отправляем сообщение
                //}
                //else
                //{
                //    Console.WriteLine("Удлилось");
                //    _clients.RemoveAt(i);
                //}
            }
        }

        private bool IsConnected(Socket socket)
        {
            Console.WriteLine(0);
            if (socket.Connected)
            {
                Console.WriteLine(1);
                if ((socket.Poll(0, SelectMode.SelectWrite)) && (!socket.Poll(0, SelectMode.SelectError)))
                {
                    Console.WriteLine(2);
                    byte[] buffer = new byte[1];
                    if (socket.Receive(buffer, SocketFlags.Peek) == 0)
                    {
                        Console.WriteLine(3);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Send(string datagram, IPEndPoint iPEndPoint)
        {
            // Создаем UdpClient
            UdpClient sender = new UdpClient();

            try
            {
                // Преобразуем данные в массив байтов
                byte[] bytes = Encoding.UTF8.GetBytes(datagram);

                // Отправляем данные
                sender.Send(bytes, bytes.Length, iPEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникло исключение: " + ex.ToString() + "\n  " + ex.Message);
            }
            finally
            {
                // Закрыть соединение
                sender.Close();
            }
        }

    }
}
