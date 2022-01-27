using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YourWarServer.Clients;

namespace YourWarServer.Chat
{
    public class ChatHandler
    {
        //private List<ClientChat> _clients = new List<ClientChat>();
        //private static TcpListener _tcpListener; // сервер для прослушивания

        //public void AddToChat(ClientChat client)
        //{
        //    _clients.Add(client);
        //}

        //protected internal void LeaveChat(int id)
        //{
        //    // получаем по id закрытое подключение
        //    ClientChat client = _clients.FirstOrDefault(c => c.ID == id);
        //    // и удаляем его из списка подключений
        //    if (client != null)
        //        _clients.Remove(client);
        //}

        //// прослушивание входящих подключений
        //protected internal void Listen()
        //{
        //    try
        //    {
        //        _tcpListener = new TcpListener(IPAddress.Any, 8888);
        //        _tcpListener.Start();
        //        Console.WriteLine("Сервер запущен. Ожидание подключений... серве по TCP");

        //        while (true)
        //        {
        //            TcpClient tcpClient = _tcpListener.AcceptTcpClient();

        //            ClientChat clientObject = new ClientChat(tcpClient, this , _clients.Count - 1);
        //            clientObject.Process();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Disconnect();
        //    }
        //}

        //// трансляция сообщения подключенным клиентам
        //protected internal void BroadcastMessage(string message, int id)
        //{
        //    byte[] data = Encoding.Unicode.GetBytes(message);
        //    for (int i = 0; i < _clients.Count; i++)
        //    {
        //        if (_clients[i].ID != id) // если id клиента не равно id отправляющего
        //        {
        //            _clients[i].Stream.Write(data, 0, data.Length); //передача данных
        //        }
        //    }
        //}
        //// отключение всех клиентов
        //protected internal void Disconnect()
        //{
        //    _tcpListener.Stop(); //остановка сервера

        //    for (int i = 0; i < _clients.Count; i++)
        //    {
        //        _clients[i].Close(); //отключение клиента
        //    }
        //    Console.WriteLine("Отвалился сервак");
        //    Console.ReadLine();
        //}
    }
}
