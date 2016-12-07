using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace HazelDash
{

    public class DashboardServer
    {
        public static readonly int PORT = 1099;
        public static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        private bool mRunning = true;
        private TcpListener mListener;

        public async void start()
        {
            mListener = new TcpListener(IPAddress.Parse("127.0.0.1"),PORT);
            mListener.Start();
            while (mRunning)
            {
                TcpClient client = await mListener.AcceptTcpClientAsync();

                connectClient(client);

                NetworkStream stream = client.GetStream();

                while (!stream.DataAvailable) ;

                byte[] encodedMessage = new byte[client.Available];
                int bytesAvailable = client.Available;

                stream.Read(encodedMessage, 0, bytesAvailable);

                string decodedMessage = decrytMessage(encodedMessage);

                runCommand(decodedMessage);
            }
        }

        private void runCommand(string message)
        {
            int spaceIndex = message.IndexOf(' ');
            string command = message.Split(' ')[0];
            if(command == "addtodo")
            {

                string todoElement = message.Substring(spaceIndex + 1, message.Length - spaceIndex - 1);
                TODOs.add(todoElement);
            }
        }

        private string decrytMessage(byte[] encoded)
        {

            int messageLength = encoded[1] - 128;
            byte[] key = new byte[4] { encoded[2], encoded[3], encoded[4], encoded[5] };

            byte[] decoded = new byte[messageLength];
            for (int i = 0; i < decoded.Length; i++)
            {
                decoded[i] = (byte)(encoded[i + 6] ^ key[i % key.Length]);
            }
            return Encoding.UTF8.GetString(decoded);
        }
        
        private void connectClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[256];

            while (!stream.DataAvailable) ;

            int bytesAvailable = client.Available;

            stream.Read(buffer, 0, bytesAvailable);

            string handshake = Encoding.UTF8.GetString(buffer);

            Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine +
                "Connection: Upgrade" + Environment.NewLine +
                "Upgrade: websocket" + Environment.NewLine +
                "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                    SHA1.Create().ComputeHash(
                        Encoding.UTF8.GetBytes(
                            new Regex("Sec-WebSocket-Key: (.*)").Match(handshake).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                            )
                        )
                    ) + Environment.NewLine +
                    Environment.NewLine);

            stream.Write(response, 0, response.Length);
        }


        private void stop() {
            mRunning = false;
            mListener.Stop();
        }
    }
}
