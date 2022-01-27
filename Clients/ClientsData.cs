using System;
using System.Collections.Generic;
using System.Net.Sockets;
using YourWarServer.Chat;

namespace YourWarServer.Clients
{

    public interface IClientsData
    {
        List<ClientChat> Clients { get; }

        void AddClient(ClientChat client);

        void RomeveClient(ClientChat client);
 

    }

}
