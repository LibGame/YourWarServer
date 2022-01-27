using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Server
{
    public class IgnoreAndFavoriteUsersCommand
    {
        private UsersDataBase _usersDataBase;

        public IgnoreAndFavoriteUsersCommand(UsersDataBase usersDataBase)
        {
            _usersDataBase = usersDataBase;
        }

        public void AddIgnoredUser(string login, string loginFavorite, string ussernameIgniored)
        {
            string sendMessage = $"{loginFavorite}/{ussernameIgniored}|";
            string previous = _usersDataBase.GetUsserDataByLogin(login, "ChatIgnoreUsers");
            _usersDataBase.UpdateDataByLogin(login, "ChatIgnoreUsers", previous + sendMessage);
        }

        public void AddFavoriteUser(string login, string loginFavorite, string ussernameFavorite)
        {
            string sendMessage = $"{loginFavorite}/{ussernameFavorite}|";
            string previous = _usersDataBase.GetUsserDataByLogin(login, "ChatFavoriteUsers");
            _usersDataBase.UpdateDataByLogin(login, "ChatFavoriteUsers", previous + sendMessage);
        }

        public void RemoveFromIgnore(string login , string item)
        {
            string previous = _usersDataBase.GetUsserDataByLogin(login, "ChatIgnoreUsers");
            var split = previous.Split('|');
            string resultToSave = "";

            foreach (var item1 in split)
            {
                if (item1 != "" && item1 != item)
                    resultToSave += item1 +"|";
            }

            _usersDataBase.UpdateDataByLogin(login, "ChatIgnoreUsers", resultToSave);

        }

        public void RemoveFromFavorite(string username, string item)
        {
            string previous = _usersDataBase.GetUsserDataByLogin(username, "ChatFavoriteUsers");
            var split = previous.Split('|');
            string resultToSave = "";
            foreach (var item1 in split)
            {
                if (item1 != item && item1 != "")
                    resultToSave += item1 + "|";
            }

            _usersDataBase.UpdateDataByLogin(username, "ChatFavoriteUsers", resultToSave);

        }

        public string GetIgnoredUsers(string ussername)
        {
            return _usersDataBase.GetUsserDataByLogin(ussername, "ChatIgnoreUsers");
        }

        public string GetFavoriteUsers(string ussername)
        {
            return _usersDataBase.GetUsserDataByLogin(ussername, "ChatFavoriteUsers");
        }

    }
}
