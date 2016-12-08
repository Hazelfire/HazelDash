﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace HazelDash
{

    public class WebsocketServer
    {
        private Action<ClientHandle> mOpenCallback;
        private Action<ClientHandle, string> mMessageCallback;
        private TcpListener mListener;
        private bool mRunning = true;

        public void setOnConnectCallback(Action<ClientHandle> callback)
        {
            mOpenCallback = callback;
        }

        public void setOnMessageCallback(Action<ClientHandle, string> callback)
        {
            mMessageCallback = callback;
        }

        public async void start(int port)
        {
            mListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            mListener.Start();

            TcpClient client = await mListener.AcceptTcpClientAsync();

            while (mRunning)
            {
                connectClient(client);
                if (mOpenCallback != null)
                    mOpenCallback(new ClientHandle(client));


                listenToClient(client);

                if (!mRunning) break;

                Task<TcpClient> connectTask = mListener.AcceptTcpClientAsync();
                connectTask.Wait();
                client = connectTask.Result;
            }
        }

        private void run(int port)
        {
            
        }

        private void listenToClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            while (client.Connected && mRunning)
            {
                if (stream.DataAvailable)
                {
                    byte[] buffer = new byte[client.Available];

                    stream.Read(buffer, 0, client.Available);

                    string message = decrytMessage(buffer);

                    if(mMessageCallback != null)
                        mMessageCallback(new ClientHandle(client), message);
                    
                }
            }
        }

         private static string decrytMessage(byte[] bytes)
        {

            string incomingData = string.Empty;
            byte secondByte = bytes[1];
            int dataLength = secondByte & 127;
            int indexFirstMask = 2;
            if (dataLength == 126)
                indexFirstMask = 4;
            else if (dataLength == 127)
                indexFirstMask = 10;

            IEnumerable<byte> keys = bytes.Skip(indexFirstMask).Take(4);
            int indexFirstDataByte = indexFirstMask + 4;

            byte[] decoded = new byte[bytes.Length - indexFirstDataByte];
            for (int i = indexFirstDataByte, j = 0; i < bytes.Length; i++, j++)
            {
                decoded[j] = (byte)(bytes[i] ^ keys.ElementAt(j % 4));
            }

            return incomingData = Encoding.UTF8.GetString(decoded, 0, decoded.Length);
        }

        private static byte[] encodeMessage(string message)
        {
            byte[] response;
            byte[] bytesRaw = Encoding.UTF8.GetBytes(message);
            byte[] frame = new byte[10];

            int indexStartRawData = -1;
            int length = bytesRaw.Length;

            frame[0] = (byte)129;
            if (length <= 125)
            {
                frame[1] = (byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (byte)126;
                frame[2] = (byte)((length >> 8) & 255);
                frame[3] = (byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (byte)127;
                frame[2] = (byte)((length >> 56) & 255);
                frame[3] = (byte)((length >> 48) & 255);
                frame[4] = (byte)((length >> 40) & 255);
                frame[5] = (byte)((length >> 32) & 255);
                frame[6] = (byte)((length >> 24) & 255);
                frame[7] = (byte)((length >> 16) & 255);
                frame[8] = (byte)((length >> 8) & 255);
                frame[9] = (byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new byte[indexStartRawData + length];

            int i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }

        private static int getEncodedMessageLength(byte[] encoded)
        {
            return encoded[1] - 128;
        }

        private static void connectClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            while (!stream.DataAvailable) ;
            byte[] buffer = new byte[client.Available];

            stream.Read(buffer, 0, client.Available);

            string handshake = Encoding.UTF8.GetString(buffer);

            byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine +
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

        public void stop()
        {
            mRunning = false;
            mListener.Stop();
        }

        // Interface for sending information to client
        public class ClientHandle
        {
            TcpClient mClient;
            public ClientHandle(TcpClient client)
            {
                mClient = client;
            }

            public void send(string message)
            {
                byte[] encoded = encodeMessage(message);
                mClient.GetStream().Write(encoded, 0, encoded.Count());
            }

            public void close()
            {
                mClient.Close();
            }

        }
    }
}
