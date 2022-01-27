using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using YourWarServer.Data.DataBases;

namespace YourWarServer.Chat
{
    public class ChatMail
    {
        private UsersDataBase _usersDataBase;

        public ChatMail(UsersDataBase usersDataBase)
        {
            _usersDataBase = usersDataBase;
        }

        private bool AddMessageRoom(string senders , string message)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("sp_InsertMessage", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlParameter sendersParam = new SqlParameter
                    {
                        ParameterName = "@senders",
                        Value = senders
                    };

                    SqlParameter date = new SqlParameter
                    {
                        ParameterName = "@date",
                        Value = DateTime.Now.ToString()
                    };
                    SqlParameter messages = new SqlParameter
                    {
                        ParameterName = "@message",
                        Value = message
                    };
                    command.Parameters.Add(sendersParam);
                    command.Parameters.Add(date);
                    command.Parameters.Add(messages);

                    command.ExecuteScalar();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        private List<MessageRoom> GetMessagesRoomByName(string user)
        {
            Console.WriteLine(1);
            try
            {

                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("sp_GetMessages", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reader = command.ExecuteReader();
                    List<MessageRoom> list = new List<MessageRoom>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                var split = reader.GetString(0).Split('/');
                                Console.WriteLine("Split " + split[0] + " " + split[1] + " " + user);
                                if (split[0] == user || split[1] == user)
                                {
                                    Console.WriteLine("Нашлись совпадения");
                                    list.Add(new MessageRoom()
                                    {
                                        Senders = reader.GetString(0),
                                        Date = reader.GetString(1),
                                        Message = reader.IsDBNull(2) ? "" : reader.GetString(2)
                                    });
                                }

                            }
                        }
                        reader.Close();
                        return list;
                    }
                    else
                    {
                        Console.WriteLine("Нет колон");
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        private void UpdateDataBySendersRoom(string senders, string row, string value)
        {
            try
            {
                string sqlExpression = String.Format("UPDATE PrivateRoomMessages SET {0} ='{1}' WHERE Senders='{2}'", row, value, senders);

                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
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

        private MessageRoom TryGetMessageRoom(string senders)
        {
            try
            {
                senders = senders.Split(' ')[0];
                string sqlExpression = String.Format("SELECT * FROM PrivateRoomMessages WHERE Senders='{0}'", senders);

                var messageRoom = new MessageRoom();
                messageRoom.Senders = senders;
                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(2)) messageRoom.Message = reader.GetString(2);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Нет колон");
                    }
                    reader.Close();
                }
                return messageRoom;
            }
            catch
            {
                return default;
            }
        }

        private bool TryCheckUsersInChatRoom(string senders)
        {
            try
            {
                senders = senders.Split(' ')[0];
                string sqlExpression = String.Format("SELECT Senders FROM PrivateRoomMessages WHERE Senders='{0}'", senders);
                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Close();
                        return true;
                    }
                    else
                    {
                        reader.Close();
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public void AddMessage(string sender, string recipient , string message)
        {

            if(DistrebuteMessageInChat(sender + "/" + recipient, sender, recipient, message) || DistrebuteMessageInChat(recipient + "/" + sender, sender, recipient, message))
            {
                return;
            }

            AddMessageRoom(sender + "/" + recipient, PrepareMessageFromNull(sender, GetNameByLogin(recipient), message));

        }

        public void AddMessageExchange(string sender, string recipient, string exchange)
        {
            AddMessageRoom(exchange + "/" + sender + "/" + recipient, PrepareMessageFromNull(sender, GetNameByLogin(recipient),"mes"));
        }

        private string GetNameByLogin(string login)
        {
            return _usersDataBase.GetUsserDataByLogin(login, "Username");
        }

        public string GetMessageRoom(string senders)
        {
            return TryGetMessageRoom(senders).Message;
        }

        public string GetRoomsInString(string user)
        {
            Console.WriteLine(1);
            string result = "";
            var messageRooms = GetMessagesRoomByName(user);
            if(messageRooms != null)
            {
                foreach (var message in messageRooms)
                {
                    result += message.Senders + "/" + message.Date + "|";
                }
            }

            return result;
        }

        private bool DistrebuteMessageInChat(string sendersInRoom, string sender, string recipient, string message)
        {
            Console.WriteLine("Distebuted " + sendersInRoom);
            if (TryCheckUsersInChatRoom(sendersInRoom))
            {
                UpdateDataBySendersRoom(sendersInRoom, "MessageChat", PrepareMessageFromData(sendersInRoom,GetNameByLogin(recipient) ,sender,message));
                return true;
            }
            else
            {
                return false;
            }
        }


        private string PrepareMessageFromNull(string sender, string recipientName, string message)
        {
            return sender + "/" + recipientName + "/" + message + "|";
        }

        private string PrepareMessageFromData(string sendersInRoom, string recipientName, string sender, string message)
        {
            return TryGetMessageRoom(sendersInRoom).Message + sender + "/" + recipientName + "/" + message + "|";
        }


    }


    public struct MessageRoom
    {
        public string Senders { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
    }
}
