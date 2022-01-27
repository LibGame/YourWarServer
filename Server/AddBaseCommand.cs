using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;
using YourWarServer.User;

namespace YourWarServer.Server
{
    public class AddBaseCommand : IClientCommand
    {
        private UsersDataBase _usersDataBase;
        private Dictionary<int, string> _basesColoumName = new Dictionary<int, string>()
        {
            [0] = "FirstBaseBattle",
            [1] = "SecondBaseBattle",
            [2] = "ThirdBaseBattle",
            [3] = "FourthBaseBattle",
            [4] = "FirstBaseTournament",
            [5] = "SecondBaseTournament",
            [6] = "ThirdBaseTournament",
            [7] = "FourthBaseTournament",
            [8] = "FirstBaseSuperTournament",
            [9] = "SecondBaseSuperTournament",
            [10] = "ThirdBaseSuperTournament",
            [11] = "FourthBaseSuperTournament",

        };

        public AddBaseCommand(UsersDataBase usersDataBase)
        {
            _usersDataBase = usersDataBase;
        }

        public string TryAddBaseByLogin(string login , int baseID ,string value)
        {
            try
            {
                _usersDataBase.UpdateDataByLogin(login, _basesColoumName[baseID], value);
                return "y";
            }
            catch
            {
                return "n";
            }
        }
    }
}
