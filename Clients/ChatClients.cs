using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Chat;

namespace YourWarServer.Clients
{
    public class ChatClients : IClientsData
    {
        private List<ClientChat> _clients = new List<ClientChat>();

        public List<ClientChat> Clients => _clients;

        List<ClientChat> IClientsData.Clients => throw new NotImplementedException();

        public void AddClient(ClientChat client)
        {
            _clients.Add(client);
        }

        public void RomeveClient(ClientChat client)
        {
            _clients.Remove(client);
        }

    }
}
