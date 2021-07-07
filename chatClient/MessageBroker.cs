using System;
using System.Net.Sockets;
using System.Text;

namespace chatClient
{
    public static class MessageBroker
    {
        public static bool getServerResponse(NetworkStream stream, out string messageResponse)
        {
            bool result = false;

            byte[] receivedBytes = new byte[1024];
            int byte_count = stream.Read(receivedBytes, 0, receivedBytes.Length);
            byte[] formated = new byte[byte_count];
            Array.Copy(receivedBytes, formated, byte_count);
            string data = Encoding.ASCII.GetString(formated);

            string[] response = data.Split('|');
            if (response.Length > 1)
            {
                if(response[0] == "1") result = true;
                messageResponse = response[1];
            }
            else
            {
                messageResponse = "Error: Unexpected message from server";
            }

            return result;
        }

        public static void sendMessageToServer(NetworkStream stream, string message)
        {
            string payload;
            
            bool valid = MessageBroker.prepareMessage(message, out payload);

            if (valid)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(payload);
                stream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                ConsoleSync.writeToConsoleSync("Invalid command. Please review and try it again!");
            }
        }

        private static bool prepareMessage(string message, out string payload)
        {
            string command = null;
            string arg = null;
            string body = null;
            bool valid = false;

            if (message.StartsWith('/'))
            {
                command = message.Substring(1, 1);

                // private message
                if (command.ToLower() == "p")
                {
                    string aux = message.Substring(message.IndexOf(" ")).Trim();
                    arg = aux.Substring(0, aux.IndexOf(" "));

                    if (arg.Length > 0)
                    {
                        body = aux.Substring(aux.IndexOf(" ")).Trim();
                        valid = true;
                    }
                }
                else if (command.ToLower() == "e") // exit
                {
                    command = "x";
                    body = message;
                    valid = true;
                }
            }
            else
            {
                command = "n"; //no command. will be ignored on the server
                body = message;                
                valid = true;
            }

            payload = command + (String.IsNullOrEmpty(arg) ? string.Empty : ("#" + arg)) + "|" + body;

            return valid;
        }
    }
}