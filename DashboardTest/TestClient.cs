using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;


using HazelDash;

namespace DashboardTest
{
    public class TestClient
    {
        ClientWebSocket mClient;
        public TestClient()
        {
            mClient = new ClientWebSocket();            
        }

        public void connect(string hostname)
        {
            mClient.ConnectAsync(new Uri("ws://" + hostname + ":" + DashboardServer.PORT.ToString()), CancellationToken.None).Wait();
        }

        public void send(string content)
        {
            byte[] encodedeMessage = Encoding.UTF8.GetBytes(content);

            mClient.SendAsync(new ArraySegment<byte>(encodedeMessage), WebSocketMessageType.Text, true, CancellationToken.None).Wait();

        }
    }
}
