using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace HazelDash
{

    public class DashboardServer
    {
        public static readonly int PORT = 1099;
        public static readonly string ADD_TODO_COMMAND = "todoadd";
        public static readonly string CLEAR_TODO_COMMAND = "todoclear";
        public static readonly string SIZE_TODO_COMMAND = "todosize";
        public static readonly string GET_TODO_COMMAND = "todoget";
        public static readonly string LAST_TODO_COMMAND = "todolast";
        public static readonly string REMOVE_TODO_COMMAND = "todoremove";


        public static readonly string ADD_TODO_SUCESS_MESSAGE = "Added todo";
        public static readonly string CLEAR_TODO_SUCCESS_MESSAGE = "Cleared todo succesfully";
        public static readonly string DELETE_TODO_SUCCESS_MESSAGE = "Successfully removed ";
        public static readonly string PARSE_UINT_ERROR = "Argument is not a valid positive integer";


        private WebsocketServer mServer;



        public void start()
        {
            mServer = new WebsocketServer();
            mServer.setOnMessageCallback(onMessage);
            mServer.start(PORT);
        }

        private void onMessage(WebsocketServer.ClientHandle client, string message)
        {
            string result;
            try {
                result = runCommand(message);
            }catch(ArgumentException e)
            {
                result = e.Message;
            }
            client.send(result);
        }

        private string runCommand(string message)
        {
            string command = message.Split(' ')[0];

            string arguments = getArguments(message);

            string result = "";
            if (command == ADD_TODO_COMMAND)
            {
                TODOs.add(arguments);
                result = ADD_TODO_SUCESS_MESSAGE;
            }
            else if (command == CLEAR_TODO_COMMAND)
            {
                TODOs.clear();
                result = CLEAR_TODO_SUCCESS_MESSAGE;
            }
            else if (command == SIZE_TODO_COMMAND)
            {
                result = TODOs.size().ToString();
            }
            else if (command == GET_TODO_COMMAND)
            {
                try
                {
                    result = TODOs.get(uint.Parse(arguments));
                }catch(Exception e)
                {
                    if (e is OverflowException || e is FormatException || e is ArgumentNullException)
                    {
                        result = PARSE_UINT_ERROR;
                    }
                }
                
            }
            else if (command == LAST_TODO_COMMAND)
            {
                result = TODOs.last();
            }
            else if (command == REMOVE_TODO_COMMAND)
            {
                try {
                    string todoToDelete = TODOs.get(uint.Parse(arguments));
                    TODOs.delete(uint.Parse(arguments));
                    result = DELETE_TODO_SUCCESS_MESSAGE + todoToDelete;
                }
                catch (Exception e)
                {
                    if (e is OverflowException || e is FormatException || e is ArgumentNullException)
                    {
                        result = PARSE_UINT_ERROR;
                    }
                }
            }
            else
            {
                result = "Command not found";
            }
            
            return result;
        }

        private string getArguments(string message)
        {
            int spaceIndex = message.IndexOf(' ');
            if (spaceIndex == -1)
            {
                return "";
            }
            else {
                return message.Substring(spaceIndex + 1, message.Length - spaceIndex - 1);
            }
        }

        public void stop() {
            mServer.stop();
        }
    }
}
