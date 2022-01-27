using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourWarServer.Tournament
{
    public class WonUser
    {
        private StructureData _structureData = new StructureData();


        public TournamentParticipant GetkWinner(TournamentParticipant firstParticipant , TournamentParticipant secondParticipant)
        {
            float powerFirst = GetParticipantPower(firstParticipant);
            float powerSecond = GetParticipantPower(secondParticipant);

            if(powerFirst > powerSecond)
            {
                return firstParticipant;
            }
            else
            {
                return secondParticipant;
            }

        }


        private float GetParticipantPower(TournamentParticipant participant)
        {
            float power = 0;
            foreach(var item in participant.Structures)
            {
                power += (_structureData.Structures[item].Damage * _structureData.Structures[item].AttackSpeed) / _structureData.Structures[item].Health;
            }

            return power;
        }
    }
}
