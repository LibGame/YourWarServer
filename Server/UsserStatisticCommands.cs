using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;
using YourWarServer.User;
using System.Data.SqlClient;
using System.Data;

namespace YourWarServer.Server
{
    public class UsserStatisticCommands
    {
        private UsersDataBase _usersDataBase;

        public UsserStatisticCommands(UsersDataBase usersDataBase)
        {
            _usersDataBase = usersDataBase;
        }

        public string GetTopWorldUsers(string login)
        {
            string topInWorld = GetTopPlayers(login, "sp_GetTopWorldUsers");
            string topInTournament = GetTopPlayers(login, "sp_GetTopTournamentUsers");
            string topInSuperTournament = GetTopPlayers(login, "sp_GetTopSuperTournamentUsers");


            return topInWorld + "|" + topInTournament + "|" + topInSuperTournament;
        }

        public string GetTopPlayers(string login , string storedProcedure)
        {
            try
            {
                string sqlExpression = storedProcedure;
                string result = "";

                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {

                    Dictionary<string, UsserStatisticItem> keyValuePairs = new Dictionary<string, UsserStatisticItem>();
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    var reader = command.ExecuteReader();
                    int id = 1;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var name = "";
                            var amount = 0;
                            var loginGetted = "";
                            if (!reader.IsDBNull(0)) loginGetted = reader.GetString(0); else loginGetted = "name1";
                            if (!reader.IsDBNull(1)) name = reader.GetString(1); else name = "name";
                            if (!reader.IsDBNull(2)) amount = reader.GetInt32(2); else amount = 0;
                            loginGetted?.Replace(" ", "");
                            name?.Replace(" ", "");

                            if (!keyValuePairs.ContainsKey(loginGetted))
                                keyValuePairs.Add(loginGetted, new UsserStatisticItem(amount, id , name));
                            id++;
                        }

                        int i = 0;
           
                        if(keyValuePairs.ContainsKey(login))
                            result += keyValuePairs[login].Username + "/" + keyValuePairs[login].AmountWins + "/" + keyValuePairs[login].Place + "/";

                        foreach (var key in keyValuePairs)
                        {
                            i++;

                            if (i > keyValuePairs.Count + 1 || i > 20)
                                break;

                            result += key.Value.Username + "/" + key.Value.AmountWins + "/" + i + "/";
                            Console.WriteLine(result);
                        }
                    }

                    reader.Close();

                }
                return result;
            }catch(Exception e)
            {
                Console.WriteLine(e);
                return "n";
            }
        }

        public void AddWins(string login , string amount)
        {
            if (_usersDataBase.GetUsserByLogin(login, out UserData userData))
            {
                _usersDataBase.UpdateData(userData.ID, "Wins", (Convert.ToInt32(amount) + userData.Wins).ToString());
            }
        }

        public void AddLoses(string login, string amount)
        {
            if (_usersDataBase.GetUsserByLogin(login, out UserData userData))
            {
                _usersDataBase.UpdateData(userData.ID, "Loses", (Convert.ToInt32(amount) + userData.Loses).ToString());
            }
        }

    }

    public struct UsserStatisticItem
    {
        public int AmountWins { get; private set; }
        public int Place { get; private set; }
        public string Username { get; private set; }

        public UsserStatisticItem(int amount , int place, string username)
        {
            AmountWins = amount;
            Place = place;
            Username = username;
        }
    }
}
