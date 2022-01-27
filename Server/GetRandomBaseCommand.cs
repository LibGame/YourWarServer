using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Server
{
    class GetRandomBaseCommand : IClientCommand
    {

        public string GetRandomBattleBase(string login)
        {
            string sqlExpression = String.Format("SELECT Login, Username, FirstBaseBattle , SecondBaseBattle , ThirdBaseBattle , FourthBaseBattle FROM Users");

            using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("Есть колонки");
                    var rnd = new Random();
                    List<string> list = new List<string>();
                    string username = "";
                    while (reader.Read())
                    {
                        if(login != reader.GetString(0))
                        {
                            if (!reader.IsDBNull(1)) username = reader.GetString(1);
                            if (!reader.IsDBNull(2)) list.Add(reader.GetString(2));
                            if (!reader.IsDBNull(3)) list.Add(reader.GetString(3));
                            if (!reader.IsDBNull(4)) list.Add(reader.GetString(4));
                            if (!reader.IsDBNull(5)) list.Add(reader.GetString(5));
                        }

                    }

                    if(list.Count > 0)
                        return $"{username}|{list[rnd.Next(0, list.Count)]}|";
                }
                reader.Close();


            }

            return "n";
        }

        public string GetRandomTournamentBase()
        {
            string sqlExpression = "sp_GetUsers";

            using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = CommandType.StoredProcedure;
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    var rnd = new Random();
                    List<string> list = new List<string>();
                    int randomColoum = rnd.Next(6, 9);
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(randomColoum)) list.Add(reader.GetString(randomColoum));
                    }

                    if (list.Count > 0)
                        return list[rnd.Next(0, list.Count - 1)];
                }

            }

            return "n";
        }

    }
}
