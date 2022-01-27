using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace YourWarServer.Chat
{
    public class ChatServer
    {
        //protected volatile List<ClientChat> _clientsGlobalChatRU = new List<ClientChat>(); // Список "подключенных" клиентов
        //protected volatile List<ClientChat> _clientsPrivateChatRU = new List<ClientChat>(); // Список "подключенных" клиентов

        //protected volatile List<ClientChat> _clientsGlobalChatENG = new List<ClientChat>(); // Список "подключенных" клиентов
        //protected volatile List<ClientChat> _clientsPrivateChatENG = new List<ClientChat>(); // Список "подключенных" клиентов

        protected Chat _chatGlobalRU = new Chat();
        protected Chat _chatPrivateRU = new Chat();
        protected Chat _chatGlobalENG = new Chat();
        protected Chat _chatPrivateENG = new Chat();


        public const int LOCAL_PORT = 8888;

        public virtual void EnterToChat()
        {

            Receiver();
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

        public virtual void Receiver()
        {
            // Создаем UdpClient для чтения входящих данных
            UdpClient receivingUdpClient = new UdpClient(LOCAL_PORT);

            IPEndPoint remoteFullIp = null;

            try
            {

                while (true)
                {
                    // Ожидание дейтаграммы
                    byte[] receiveBytes = receivingUdpClient.Receive(
                       ref remoteFullIp);

                    // Преобразуем и отображаем данные
                    string returnData = Encoding.UTF8.GetString(receiveBytes);
                    string[] splitMessage = returnData.Split('/');
                    if (splitMessage[0] == "Exit$")
                    {
                        Console.WriteLine("splitMessage[3] " + splitMessage[3]);
                        if (splitMessage[3] == "ENG")
                        {
                            Console.WriteLine("splitMessage[2] " + splitMessage[2]);
                            if (splitMessage[2] == "Global")
                            {
                                _chatGlobalENG.Remove(splitMessage[1], remoteFullIp);
                            }
                            else
                            {
                                _chatPrivateENG.Remove(splitMessage[1], remoteFullIp);
                            }
                        }
                        else
                        {
                            Console.WriteLine("splitMessage[2] " + splitMessage[2]);

                            if (splitMessage[2] == "Global")
                            {
                                _chatGlobalRU.Remove(splitMessage[1], remoteFullIp);
                            }
                            else
                            {
                                _chatPrivateRU.Remove(splitMessage[1], remoteFullIp);
                            }
                        }
                        Console.WriteLine("Ливнул");

                    }
                    else
                    {

                        if (splitMessage[4] == "ENG")
                        {
                            Console.WriteLine("CHAT MESSAGE ENG");
                            if (splitMessage[0] == "Global")
                            {
                                _chatGlobalENG.PrepareMessage(splitMessage[1], remoteFullIp, splitMessage[1] + "/" + splitMessage[2] + "/" + splitMessage[3], splitMessage[1]);
                            }
                            else
                            {
                                _chatPrivateENG.PrepareMessage(splitMessage[1], remoteFullIp, splitMessage[1] + "/" + splitMessage[2] + "/" + splitMessage[3], splitMessage[1]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("CHAT MESSAGE RU");
                            if (splitMessage[0] == "Global")
                            {
                                _chatGlobalRU.PrepareMessage(splitMessage[1], remoteFullIp, splitMessage[1] + "/" + splitMessage[2] + "/" + splitMessage[3], splitMessage[1]);
                            }
                            else
                            {
                                _chatPrivateRU.PrepareMessage(splitMessage[1], remoteFullIp, splitMessage[1] + "/" + splitMessage[2] + "/" + splitMessage[3], splitMessage[1]);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникло исключение: " + ex.ToString() + "\n  " + ex.Message);
            }
        }


        //    private void PrepareMessageGlobal(string login, IPEndPoint remoteFullIp, string message , string username)
        //    {
        //        if (!CheckIfUsserInGlobalClinetsList(login))
        //        {
        //            Console.WriteLine("Добавили " + remoteFullIp.Address.ToString());
        //            _clientsGlobalChatRU.Add(new ClientChat(remoteFullIp, login, username));
        //            BroadcastMessageGlobal(message);
        //        }
        //        else
        //        {
        //            BroadcastMessageGlobal(message);
        //        }

        //    }

        //    private void PrepareMessagePrivate(string usserName, IPEndPoint remoteFullIp, string message, string login)
        //    {
        //        if (!CheckIfUsserInPrivateClinetsList(usserName))
        //        {
        //            Console.WriteLine("Добавили " + remoteFullIp.Address.ToString());
        //            _clientsPrivateChatRU.Add(new ClientChat(remoteFullIp, usserName , login));
        //            BroadcastMessagePrivate(message);
        //        }
        //        else
        //        {
        //            BroadcastMessagePrivate(message);
        //        }

        //    }

        //    public virtual bool CheckIfUsserInGlobalClinetsList(string ussername)
        //    {
        //        for (int i = 0; i < _clientsGlobalChatRU.Count; i++)
        //        {
        //            if (_clientsGlobalChatRU[i].Login == ussername)
        //            {
        //                return true;
        //            }

        //        }
        //        return false;
        //    }

        //    public virtual bool CheckIfUsserInPrivateClinetsList(string ussername)
        //    {
        //        for (int i = 0; i < _clientsPrivateChatRU.Count; i++)
        //        {
        //            if (_clientsPrivateChatRU[i].Login == ussername)
        //            {
        //                return true;
        //            }

        //        }
        //        return false;
        //    }


        //    // Метод для рассылки сообщений
        //    public virtual void BroadcastMessageGlobal(string message)
        //    {
        //        for (int i = 0; i < _clientsGlobalChatRU.Count; i++) // Циклом перебераем всех клиентов
        //        {
        //            Send(message, _clientsGlobalChatRU[i].IPEndPoint); // Отправляем сообщение
        //        }
        //    }

        //    public virtual void BroadcastMessagePrivate(string message)
        //    {
        //        for (int i = 0; i < _clientsPrivateChatRU.Count; i++) // Циклом перебераем всех клиентов
        //        {
        //            Send(message, _clientsPrivateChatRU[i].IPEndPoint); // Отправляем сообщение
        //        }
        //    }

        //    public virtual void DeleteUsserByName(string name)
        //    {
        //        for (int i = 0; i < _clientsGlobalChatRU.Count; i++)
        //        {
        //            if (_clientsGlobalChatRU[i].Login == name)
        //                _clientsGlobalChatRU.RemoveAt(i);
        //        }
        //    }
        //}

    }
}
