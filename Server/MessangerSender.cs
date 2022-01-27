using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace YourWarServer.Server
{
    public class MessangerSender
    {

        public void SendMessage(string message , TcpClient tcpClient)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(buffer, 0, buffer.Length);
        }

        public void SendBytes(byte[] bytes, TcpClient tcpClient)
        {
            using (var fs = new MemoryStream())
            {

                fs.Write(bytes, 0, bytes.Length);
                fs.Seek(0, SeekOrigin.Begin);
                byte[] buffer;
                NetworkStream networkSteam = tcpClient.GetStream();

                int bufferSize = 100000;
                int bufferCount = Convert.ToInt32(Math.Ceiling((double)fs.Length / (double)bufferSize));
                int size;
                for (int i = 0; i < bufferCount; i++)
                {
                    buffer = new byte[bufferSize];
                    size = fs.Read(buffer, 0, bufferSize);

                    networkSteam.Write(buffer, 0, size);
                }

                var headerBytes = Encoding.UTF8.GetBytes("end");
                networkSteam.Write(headerBytes, 0, headerBytes.Length);

            }
        }
    }
}
