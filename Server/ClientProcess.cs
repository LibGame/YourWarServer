using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YourWarServer.Data.DataBases;


namespace YourWarServer.Server
{
    public class ClientProcess
    {

        private static bool _isNextImage;
        private static string _login;
        private static int _length;
        private static List<byte> _bytes = new List<byte>();

        public static async void StartProcessAsync(TcpClient client, ClientCommands clientCommands)
        {
            Console.WriteLine(1);
            await Task.Run(() => Process(client , clientCommands));
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        public static byte[] ReciveBytes(NetworkStream networkStream , int byteSize)
        {
            byte[] readBuffer = new byte[byteSize];

            using (var writer = new MemoryStream())
            {
                while (networkStream.DataAvailable)
                {
                    int numberOfBytesRead = networkStream.Read(readBuffer, 0, readBuffer.Length);
                    if (numberOfBytesRead <= 0)
                    {
                        break;
                    }
                    writer.Write(readBuffer, 0, numberOfBytesRead);
                }

                return writer.ToArray();
            }
        }

        public static void AddToData(byte[] data)
        {
            try
            {
                string sqlExpression = String.Format("UPDATE Users SET Icon = @icon WHERE Login='{0}'", _login);
                Console.WriteLine(10);

                using (SqlConnection connection = new SqlConnection(UsersDataBase.ConnecionPath))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.Parameters.Add(new SqlParameter("@icon", data));

                    Console.WriteLine("Длина записи " + data.Length);
                    int number = command.ExecuteNonQuery();
                }
                Console.WriteLine(11);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static bool IsConnected(TcpClient client)
        {
            if (client.Client.Connected)
            {
                if ((client.Client.Poll(0, SelectMode.SelectWrite)) && (!client.Client.Poll(0, SelectMode.SelectError)))
                {
                    byte[] buffer = new byte[1];
                    if (client.Client.Receive(buffer, SocketFlags.Peek) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void Process(TcpClient client , ClientCommands clientCommands)
        {
            string data;
            clientCommands.OnRedyToLoadIcon += SetIsNextImageAndLogin;
            Console.WriteLine("Подключилось");
            while (true)
            {
                try
                {
                    if (!IsConnected(client))
                    {
                        Console.WriteLine("Оборвалось");
                        break;
                    }
                    NetworkStream stream = client.GetStream();

                    if (_isNextImage)
                    {

                        Encoding utf8 = new UTF8Encoding(false);
                        long length;
                        using (BinaryReader br = new BinaryReader(stream, utf8, true))
                        {
                            length = br.ReadInt64();
                        }

                        using (var fs = new MemoryStream())
                        {
                            byte[] buffer = new byte[1024];
                            long received = 0;
                            while (received < length)
                            {
                                int toReceive = (int)Math.Min(buffer.Length, length - received);
                                int bytesReceived = stream.Read(buffer, 0, toReceive);
                                if (bytesReceived == 0) // Неожиданный конец потока
                                    return;
                                received += bytesReceived;
                                fs.Write(buffer, 0, bytesReceived);
                            }
                            data = Encoding.UTF8.GetString(fs.ToArray(), 0, (int)received);
                            AddToData(fs.ToArray());
                            _isNextImage = false;

                            clientCommands.MessangerSender.SendMessage("end", client);
                        }
                    }
                    else
                    {
                        Encoding utf8 = new UTF8Encoding(false);
                        long length;
                        using (BinaryReader br = new BinaryReader(stream, utf8, true))
                        {
                            length = br.ReadInt64();
                        }

                        using (var fs = new MemoryStream())
                        {
                            byte[] buffer = new byte[1024];
                            long received = 0;
                            while (received < length)
                            {
                                int toReceive = (int)Math.Min(buffer.Length, length - received);
                                int bytesReceived = stream.Read(buffer, 0, toReceive);
                                if (bytesReceived == 0) // Неожиданный конец потока
                                    return;
                                received += bytesReceived;
                                fs.Write(buffer, 0, bytesReceived);
                            }
                            data  = Encoding.UTF8.GetString(fs.ToArray(), 0, (int)received);
                            Console.WriteLine("Data " + data);
                            if ((int)received > 0)
                            {
                                CommandAndMessages commandAndMessages = GetReciveCommandAndMessage(data);
                                clientCommands.UseCommand(commandAndMessages, client);
                            }
                        }

                        //byte[] buffer = new byte[100000];
                        //int byte_count;

                        //byte_count = stream.Read(buffer, 0, buffer.Length);
                        //data = Encoding.UTF8.GetString(buffer, 0, byte_count);

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
 


            }
            _isNextImage = false;
            _login = "";
            _bytes.Clear();
            Console.WriteLine("Вышел");
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private void ReceiveFile(Stream stream)
        {

        }

        private static bool IsEnd(byte[] buffer)
        {
            string result = Encoding.UTF8.GetString(buffer);
            Console.WriteLine("Result " + result);
            if (result.Split(' ')[0] == "end")
                return true;

            return false;
        }

        public static void SetIsNextImageAndLogin(string login)
        {
            Console.WriteLine("Может записывать картину");
            _isNextImage = true;
            _login = login;
        }


        public static CommandAndMessages GetReciveCommandAndMessage(string message)
        {
            var list = message.ToCharArray();
            string resultWord = "";
            List<string> messages = new List<string>();
            string command = "";

            bool isGetedCommand = false;

            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].ToString() == "|")
                {
                    if (isGetedCommand)
                    {
                        Console.WriteLine(resultWord);
                        messages.Add(resultWord);
                    }
                    else
                    {
                        command = resultWord;
                        isGetedCommand = true;
                    }
                    resultWord = "";
                }
                else
                {
                    resultWord += list[i].ToString();
                }
            }
            return new CommandAndMessages(command, messages.ToArray());
        }
    }

}
