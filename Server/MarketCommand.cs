using System;
using System.Collections.Generic;
using YourWarServer.Data.DataBases;
using System.Data.SqlClient;
using System.Data;
using YourWarServer.User;
using YourWarServer.Chat;

namespace YourWarServer.Server
{
    class MarketCommand : IClientCommand
    {
        private UsersDataBase _usersDataBase;
        private ClientCommands _clientCommands;
        private ChatMail _chatMail;

        public MarketCommand(UsersDataBase usersDataBase , ClientCommands clientCommands , ChatMail chatMail)
        {
            _usersDataBase = usersDataBase;
            _clientCommands = clientCommands;
            _chatMail = chatMail;
        }
        public string AddDealToUser(string sellerLogin, string userTo, string productID, string productAmount, string offerProductID, string offerProductAmount)
        {
            string sqlExpression = "sp_InsertProduct";

            try
            {
                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(CreateParamets("@sellerLogin", sellerLogin));
                    command.Parameters.Add(CreateParamets("@productId", productID));
                    command.Parameters.Add(CreateParamets("@productAmount", productAmount));
                    command.Parameters.Add(CreateParamets("@offerProductID", offerProductID));
                    command.Parameters.Add(CreateParamets("@offerProductAmount", offerProductAmount));
                    command.Parameters.Add(CreateParamets("@isExchangeMarket", "1"));


                    command.ExecuteNonQuery();
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT @@IDENTITY";
                    int lastId = Convert.ToInt32(command.ExecuteScalar());
                    Console.WriteLine("LastID " + lastId);
                    _chatMail.AddMessageExchange(sellerLogin, userTo, $"&%{lastId}");
                    return "y";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "n";
            }
        }


        public string GetProductByID(string id)
        {
            string result = "";
            string sqlExpression = String.Format("SELECT * FROM Market WHERE Id={0}", id);

            try
            {
                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0)) result += $"{reader.GetInt32(0)}/";
                            if (!reader.IsDBNull(1)) result += $"{reader.GetString(1)}/";
                            if (!reader.IsDBNull(2)) result += $"{reader.GetInt32(2)}/";
                            if (!reader.IsDBNull(3)) result += $"{reader.GetInt32(3)}/";
                            if (!reader.IsDBNull(4)) result += $"{reader.GetInt32(4)}/";
                            if (!reader.IsDBNull(5)) result += $"{reader.GetInt32(5)}/";
                        }
                        return result;
                    }
                    else
                    {
                        reader.Close();
                        return "n";
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "n";
            }

        }

        public string AddDeal(string sellerLogin, string productID , string productAmount , string offerProductID , string offerProductAmount)
        {
            string sqlExpression = "sp_InsertProduct";

            try
            {
                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(CreateParamets("@sellerLogin", sellerLogin));
                    command.Parameters.Add(CreateParamets("@productId", productID));
                    command.Parameters.Add(CreateParamets("@productAmount", productAmount));
                    command.Parameters.Add(CreateParamets("@offerProductID", offerProductID));
                    command.Parameters.Add(CreateParamets("@offerProductAmount", offerProductAmount));
                    command.Parameters.Add(CreateParamets("@isExchangeMarket", "0"));


                    command.ExecuteNonQuery();

                    Console.WriteLine("Добавлен продукт");
                    return "y";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "n";
            }
        }


        public string GetProducts()
        {
            string result = "";
            string sqlExpression = "sp_GetProducts";

            try
            {
                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0)) result += $"{reader.GetInt32(0)}/";
                            if (!reader.IsDBNull(1)) result += $"{reader.GetString(1)}/";
                            if (!reader.IsDBNull(2)) result += $"{reader.GetInt32(2)}/";
                            if (!reader.IsDBNull(3)) result += $"{reader.GetInt32(3)}/";
                            if (!reader.IsDBNull(4)) result += $"{reader.GetInt32(4)}/";
                            if (!reader.IsDBNull(5)) result += $"{reader.GetInt32(5)}/";
                            result += "|";
                        }
                    }
                    reader.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;

        }

        public string BuyProduct(string idAd , string usserSeller, string idProduct , string amount)
        {
            try
            {

                Console.WriteLine("ID " + idAd);
                string sqlExpression = String.Format("DELETE from Market WHERE Id={0}", idAd);

                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    int number = command.ExecuteNonQuery();
                    Console.WriteLine("Удалено: {0}", number);

                    if (TrySendCurrencySeller(usserSeller, Convert.ToInt32(idProduct), Convert.ToInt32(amount)) == "y")
                        return "y";
                    else
                        return "n";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "n";
            }
        }

        private SqlParameter CreateParamets(string parametrName , string value)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = parametrName,
                Value = value
            };
            return param;
        }

        private string TrySendCurrencySeller(string usserLogin ,int id , int amount)
        {
            if(id == 0)
            {
                return _clientCommands.WalletCommands.AddCupsCommand(usserLogin, amount.ToString());
            }else if(id == 1)
            {
                return _clientCommands.WalletCommands.AddBattlePassCommand(usserLogin, amount.ToString());
            }
            else if (id == 2)
            {
                return _clientCommands.WalletCommands.AddMedalsCommand(usserLogin, amount.ToString());
            }
            else if (id == 3)
            {
                return _clientCommands.WalletCommands.AddPatronsCommand(usserLogin, amount.ToString());
            }
            else
            {
                return _clientCommands.InventoryCommand.TryAddToInventoryShirk(usserLogin, $"{id}/{amount}/");
            }
        }
    }
}
