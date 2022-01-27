using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;
using YourWarServer.User;

namespace YourWarServer.Server
{
    public class WalletCommands : IClientCommand
    {
        private UsersDataBase _usersDataBase;

        public WalletCommands(UsersDataBase usersDataBase)
        {
            _usersDataBase = usersDataBase;
        }

        public string AddCupsCommand(string login, string value)
        {
            return AddDataInWallet(login, "Cups", value);
        }

        public string AddBattlePassCommand(string login, string value)
        {
            return AddDataInWallet(login, "BattlePass", value);
        }

        public string AddMedalsCommand(string login, string value)
        {
            return AddDataInWallet(login, "Medals", value);
        }

        public string AddPatronsCommand(string login, string value)
        {
            return AddDataInWallet(login, "Patrons", value);
        }

        public string GetCupsCommand(string login)
        {
            try
            {
                if (_usersDataBase.GetUsserByLogin(login, out UserData userData))
                    return userData.Cups.ToString();
                else
                    return "n";
            }
            catch
            {
                return "n";
            }
        }
        public string GetBattlePassCommand(string login)
        {
            try
            {
                if (_usersDataBase.GetUsserByLogin(login, out UserData userData))
                    return userData.BattlePass.ToString();
                else
                    return "n";
            }
            catch
            {
                return "n";
            }
        }

        public string GetMedalsCommand(string login)
        {
            try
            {
                if (_usersDataBase.GetUsserByLogin(login, out UserData userData))
                    return userData.Medals.ToString();
                else
                    return "n";
            }
            catch
            {
                return "n";
            }
        }

        public string GetPatronsCommand(string login)
        {
            try
            {
                if (_usersDataBase.GetUsserByLogin(login, out UserData userData))                                
                    return userData.Patrons.ToString();           
                else               
                    return "n";          
            }
            catch
            {
                return "n";
            }
        }

        private string AddDataInWallet(string login, string row, string value)
        {
            try
            {
                if (_usersDataBase.GetUsserByLogin(login, out UserData userData))
                {
                    _usersDataBase.UpdateData(userData.ID, row, value);
                    return "y";
                }
                else
                {
                    return "n";
                }
            }
            catch
            {
                return "n";
            }
        }
    }
}
