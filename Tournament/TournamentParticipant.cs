using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace YourWarServer.Tournament
{
    public struct TournamentParticipant
    {
        private List<int> _structuresID;

        public int BaseID { get; private set; }
        public string BaseSID { get; private set; }
        public string Login { get; private set; }
        public IReadOnlyCollection<int> Structures => _structuresID;
       
        public TournamentParticipant(List<int> structures , string baseSID ,string login , int baseID)
        {
            _structuresID = structures;
            BaseSID = baseSID;
            Login = login;
            BaseID = baseID;

        }

    }
}
