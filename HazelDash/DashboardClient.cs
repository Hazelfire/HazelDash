using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HazelDash
{
    public class DashboardClient
    {
        WebsocketClient mClient;

        public DashboardClient(string hostname)
        {
            mClient = new WebsocketClient();
            connect(hostname);
        }

        public void TODOClear()
        {
            mClient.send(DashboardServer.CLEAR_TODO_COMMAND);
            string result = mClient.getNextMessage();
            if (result != DashboardServer.CLEAR_TODO_SUCCESS_MESSAGE)
            {
                throw new ArgumentException(result);
            }
        }

        public void TODOAdd(string todoElement)
        {
            mClient.send(DashboardServer.ADD_TODO_COMMAND + " " + todoElement);
            string result = mClient.getNextMessage();
            if (result != DashboardServer.ADD_TODO_SUCESS_MESSAGE)
            {
                throw new ArgumentException(result);
            }
                        
        }

        public string TODOGet(int index)
        {
            mClient.send(DashboardServer.GET_TODO_COMMAND + " " + index);
            string result = mClient.getNextMessage();
            return result;
        }

        public int TODOSize()
        {
            mClient.send(DashboardServer.SIZE_TODO_COMMAND);
            string result = mClient.getNextMessage();
            
            return int.Parse(result);
        }

        public string TODOLast()
        {
            mClient.send(DashboardServer.LAST_TODO_COMMAND);
            string result = mClient.getNextMessage();
            return result;
        }

        public void close()
        {
            mClient.close();
        }

        public void connect(string hostname)
        {
            mClient.connect(hostname);
        }
    }
}
