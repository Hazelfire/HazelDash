using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;


using HazelDash;

namespace HazelDash
{
    public class WebsocketClient
    {
        ClientWebSocket mClient;

        private static int BUFFUR_SIZE = 1024;

        public WebsocketClient()
        {
                
        }

        public void connect(string hostname)
        {
            string uri = "ws://" + hostname + ":" + DashboardServer.PORT.ToString();
            mClient = new ClientWebSocket();
            mClient.ConnectAsync(new Uri(uri), CancellationToken.None).Wait();
        }

        public void send(string content)
        {
            byte[] encodedeMessage = Encoding.UTF8.GetBytes(content);

            mClient.SendAsync(new ArraySegment<byte>(encodedeMessage), WebSocketMessageType.Text, true, CancellationToken.None).Wait();

        }

        public void close()
        {
            mClient.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).Wait();
            
        }

        public string getNextMessage()
        {
            byte[] buffer = new byte[BUFFUR_SIZE];

            ArraySegment<byte> bufferSegment = new ArraySegment<byte>(buffer);

            Task<WebSocketReceiveResult> recieveTask = mClient.ReceiveAsync(bufferSegment, CancellationToken.None);
            recieveTask.Wait();

            WebSocketReceiveResult result = recieveTask.Result;

            int offset = result.Count;
            if (result.MessageType == WebSocketMessageType.Text)
            {
                while (!result.EndOfMessage)
                {
                    ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                    recieveTask = mClient.ReceiveAsync(bufferSegment, CancellationToken.None);
                    recieveTask.Wait();

                    result = recieveTask.Result;
                }
            }

            string message = Encoding.UTF8.GetString(buffer.ToArray(), 0, offset);
            return message;
            
        }
    }
}
