using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourWarServer.User;

namespace YourWarServer.Data.DataBases
{
    public class UsersDataBase : DataBase
    {

        private SqlConnection _sqlConnection;
        public static readonly string ConnecionPath = @"Data Source=WIN-7FHSKIQJ2UH;Initial Catalog=Users;Integrated Security=True";

        public override SqlConnection SqlConnection => _sqlConnection;

        public override void CreateDataBase()
        {
            //Console.WriteLine("1");
            //string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //_sqlConnection = new SqlConnection(_connecionPath);
            //Console.WriteLine("2");

            //try
            //{
            //    Console.WriteLine("3");
            //    _sqlConnection.Open();
            //    _sqlCommand = new SqlCommand(_userInsertProcedure, _sqlConnection);
            //    Console.WriteLine("6");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("4");
            //    Console.WriteLine(e.Message);
            //}
        }


        public bool AddUser(string login)
        {
            try
            {
                string sqlExpression = "sp_InsertUser";

                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlParameter nameParam = new SqlParameter
                    {
                        ParameterName = "@Login",
                        Value = login
                    };
                    command.Parameters.Add(nameParam);

                    var result = command.ExecuteScalar();

                    Console.WriteLine("Id добавленного объекта: {0}", result);
                }

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public void AddIconByLogin(string login , string value)
        {
            try
            {
                string sqlExpression = String.Format("UPDATE Users SET Icon = @icon WHERE Id={0}", login);

                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.Parameters.Add(new SqlParameter("@icon", value));


                    int number = command.ExecuteNonQuery();
                    Console.WriteLine("Обновлено объектов: {0}", number);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void UpdateData(int id , string row , string value)
        {
            try
            {
                string sqlExpression = String.Format("UPDATE Users SET {0} ='{1}' WHERE Id={2}", row, value, id);

                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    int number = command.ExecuteNonQuery();
                    Console.WriteLine("Обновлено объектов: {0}", number);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DeleteRowByName(string username , string coloum)
        {
            try
            {
                string sqlExpression = String.Format("UPDATE Users SET {0} ='' WHERE Login='{1}'", coloum, username);

                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    int number = command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public void UpdateDataByLogin(string login, string row, string value)
        {
            try
            {
                string sqlExpression = String.Format("UPDATE Users SET {0} ='{1}' WHERE Login='{2}'", row, value, login);

                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    int number = command.ExecuteNonQuery();
                    Console.WriteLine("Обновлено объектов: {0}", number);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public int GetIntValueByLogin(string login, string colums)
        {
            try
            {
                string sqlExpression = String.Format("SELECT {0} FROM Users WHERE Login='{1}'", colums, login);
                int result = 0;
                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0)) result += reader.GetInt32(0); else result = 0;
                        }
                    }
                    reader.Close();
                }
                return result;
            }
            catch
            {
                return 0;
            }
        }

        public string GetUsserDataByLogin(string login , string colums , string table = "Users" , string findType = "Login")
        {
            string sqlExpression = String.Format("SELECT {0} FROM {1} WHERE {2}='{3}'", colums,table , findType, login);
            string result = "";


            using (SqlConnection connection = new SqlConnection(ConnecionPath))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0)) result += reader.GetString(0); else result += "";
                    }
                }
                else
                {
                    Console.WriteLine("Нет колон");
                }
                reader.Close();
            }
            return result;

        }

        public void DeleteUserByLogin(string login)
        {
            try
            {
                string sqlExpression = String.Format("DELETE from Users WHERE Login='{0}'", login);

                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    int number = command.ExecuteNonQuery();
                    Console.WriteLine("Удалено объектов: {0}", number);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public byte[] GetBytesByLogin(string login, string colums)
        {
            try
            {
                string sqlExpression = String.Format("SELECT {0} FROM Users WHERE Login='{1}'", colums, login);
                byte[] result = new byte[0];
                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return (byte[])reader[0];
                        }
                    }
                    reader.Close();
                }
                return result;
            }
            catch
            {
                return new byte[0];
            }

        }

        public string GetValueByLogin(string login, string colums , bool isString = true)
        {
            try
            {
                string sqlExpression = String.Format("SELECT {0} FROM Users WHERE Login='{1}'", colums, login);
                string result = "";
                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (isString)
                            {
                                if (!reader.IsDBNull(0)) result += reader.GetString(0); else result += "";
                            }
                            else
                            {
                                if (!reader.IsDBNull(0)) result += reader.GetInt32(0); else result += "";
                            }
                        }
                    }
                    reader.Close();
                }
                return result;
            }
            catch
            {
                return "";
            }

        }

        public void AddBaseInTorunament(string login , string value)
        {
            try
            {
                string previousValue = GetValueByLogin(login, "BaseInTorunament");
                string sqlExpression = String.Format("UPDATE Users SET BaseInTorunament ='{0}' WHERE Login='{1}'", login, previousValue + value + "|");

                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AddParticeToTournament(string login , string value)
        {
            try
            {
                string previousValue = GetValueByLogin(login, "ParticipatesInTournament");
                UpdateDataByLogin(login, "ParticipatesInTournament", previousValue + value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AddParticeToSuperTournament(string login, string value)
        {
            try
            {
                string previousValue = GetValueByLogin(login, "ParticipatesInSuperTournament");
                UpdateDataByLogin(login, "ParticipatesInSuperTournament", previousValue + value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool GetUsserByLogin(string login, out UserData userData)
        {
            try
            {

                string sqlExpression = String.Format("SELECT * FROM Users WHERE Login='{0}'", login);


                using (SqlConnection connection = new SqlConnection(ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        Console.WriteLine("Есть колоны");
                        while (reader.Read())
                        {
                            userData = new UserData(
                                                reader.IsDBNull(0) ? -1 : reader.GetInt32(0),
                                                reader.IsDBNull(1) ? "" : reader.GetString(1),
                                                reader.IsDBNull(2) ? "" : reader.GetString(2),
                                                reader.IsDBNull(3) ? "" : reader.GetString(3),
                                                reader.IsDBNull(4) ? "" : reader.GetString(4),
                                                reader.IsDBNull(5) ? "" : reader.GetString(5),
                                                reader.IsDBNull(6) ? "" : reader.GetString(6),
                                                reader.IsDBNull(7) ? "" : reader.GetString(7),
                                                reader.IsDBNull(8) ? "" : reader.GetString(8),
                                                reader.IsDBNull(9) ? "" : reader.GetString(9),
                                                reader.IsDBNull(10) ? "" : reader.GetString(10),
                                                reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                                                reader.IsDBNull(12) ? 0 : reader.GetInt32(12),
                                                reader.IsDBNull(13) ? 0 : reader.GetInt32(13),
                                                reader.IsDBNull(14) ? 0 : reader.GetInt32(14),
                                                reader.IsDBNull(15) ? 0 : reader.GetInt32(15),
                                                reader.IsDBNull(16) ? 0 : reader.GetInt32(16),
                                                reader.IsDBNull(17) ? "" : reader.GetString(17),
                                                reader.IsDBNull(18) ? "" : reader.GetString(18));
                                                
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Нет колон");
                    }
                    reader.Close();
                    userData = default;
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                userData = default;
                return false;
            }
        }
    }
}
