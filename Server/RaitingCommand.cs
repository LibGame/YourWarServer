using System;
using System.Data;
using System.Data.SqlClient;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Server
{
    public class RaitingCommand
    {
     

        public void SortTopInWorld()
        {
            try
            {
                string sqlExpression = "SELECT Username FROM Users ORDER BY Wins DESC";

                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath)) 
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader.GetString(1));
                        }
                    }
                    reader.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
