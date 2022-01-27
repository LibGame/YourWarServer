using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Tournament
{
    public class TournamentDistributor
    {

        private Tournament _tournament50Players;
        private Tournament _tournament100Players;
        private Tournament _tournament250Players;
        private Tournament _tournament500Players;

        private SuperTournament _superTournament50Players;
        private SuperTournament _superTournament100Players;
        private SuperTournament _superTournament250Players;
        private SuperTournament _superTournament500Players;

        private int _tournamentID = 0;

        public TournamentDistributor(UsersDataBase usersDataBase)
        {
            _tournament50Players = new Tournament(usersDataBase,50, this);
            _tournament100Players= new Tournament(usersDataBase, 100, this);
            _tournament250Players= new Tournament(usersDataBase, 250, this);
            _tournament500Players = new Tournament(usersDataBase, 500, this);

            _superTournament50Players =  new SuperTournament(usersDataBase, 50, this);
            _superTournament100Players = new SuperTournament(usersDataBase, 100, this);
            _superTournament250Players = new SuperTournament(usersDataBase, 250, this);
            _superTournament500Players = new SuperTournament(usersDataBase, 500, this);
        }

        public string AddParticipant(string tourament ,string baseSID, string login , int baseID)
        {
            try
            {
                switch (tourament)
                {
                    case "Tournament50Players":
                        _tournament50Players.AddParticipant(new TournamentParticipant(ConvertBaseSIDToList(baseSID), baseSID, login, baseID));
                        return "y";
                    case "Tournament100Players":
                        _tournament100Players.AddParticipant(new TournamentParticipant(ConvertBaseSIDToList(baseSID), baseSID, login, baseID));
                        return "y";
                    case "Tournament250Players":
                        _tournament250Players.AddParticipant(new TournamentParticipant(ConvertBaseSIDToList(baseSID), baseSID, login, baseID));
                        return "y";
                    case "Tournament500Players":
                        _tournament500Players.AddParticipant(new TournamentParticipant(ConvertBaseSIDToList(baseSID), baseSID, login, baseID));
                        return "y";
                    case "SuperTournament50Players":
                        _superTournament50Players.AddParticipant(new TournamentParticipant(ConvertBaseSIDToList(baseSID), baseSID, login, baseID));
                        return "y";
                    case "SuperTournament100Players":
                        _superTournament100Players.AddParticipant(new TournamentParticipant(ConvertBaseSIDToList(baseSID), baseSID, login, baseID));
                        return "y";
                    case "SuperTournament250Players":
                        _superTournament250Players.AddParticipant(new TournamentParticipant(ConvertBaseSIDToList(baseSID), baseSID, login, baseID));
                        return "y";
                    case "SuperTournament500Players":
                        _superTournament500Players.AddParticipant(new TournamentParticipant(ConvertBaseSIDToList(baseSID), baseSID, login, baseID));
                        return "y";

                }
                return "n";

            }
            catch
            {
                return "n";
            }
        }

        public int GetTournamentID()
        {
            _tournamentID++;
            return _tournamentID - 1;
        }

        public List<int> ConvertBaseSIDToList(string baseSID)
        {
            var result = new List<int>();

            try
            {
                char[] sidSymbols = baseSID.ToCharArray();
                string id = "";
                int amount = 0;

                foreach (var symbol in sidSymbols)
                {
                    if (symbol.ToString() != "/")
                    {
                        if (amount == 0)
                        {
                            id += int.Parse(symbol.ToString()).ToString();
                        }
                    }
                    else
                    {
                        if (amount >= 2)
                        {

                            result.Add(Convert.ToInt32(id));
                            id = "";
                            amount = 0;
                        }
                        else
                        {
                            amount++;
                        }
                    }
                }
                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}
