using System;
using System.Net.Sockets;
using System.Text;

namespace chatClient
{

    public static class MessageBroker
    {

        /// <summary>
        /// Reads response from the socket data stream.
        /// </summary>
        /// <param name="stream"> TCP socket stream for communication. </param>
        /// <param name="messageResponse"> Response message received. </param>
        /// <returns>< 'True'if it was possible to process the message. /returns>
        public static bool getServerResponse(NetworkStream stream, out string messageResponse)
        {
            bool result = false;

            // reads data stream from socket
            byte[] receivedBytes = new byte[1024];
            int byte_count = stream.Read(receivedBytes, 0, receivedBytes.Length);
            byte[] formated = new byte[byte_count];
            Array.Copy(receivedBytes, formated, byte_count);
            string payload = Encoding.ASCII.GetString(formated);

            result = MessageBroker.processPayloadReceived(payload, out messageResponse);

            return result;
        }

        /// <summary>
        /// Writes message to server into the data stream.
        /// </summary>
        /// <param name="stream"> TCP socket stream for communication. </param>
        /// <param name="userMessage"> User console entry </param>
        public static void sendMessageToServer(NetworkStream stream, string userMessage)
        {
            string payload = null;
            
            bool valid = MessageBroker.preparePayloadToSend(userMessage, out payload);

            if (valid)
            {
                // writes data stream into socket
                byte[] buffer = Encoding.ASCII.GetBytes(payload);
                stream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                Console.WriteLine("Invalid command. Please review and try it again!");
            }
        }

        /// <summary>
        /// Process a message received from the server.
        /// </summary>
        /// <param name="payload"> String payload received from the server. </param>
        /// <param name="message"> Body message received </param>
        /// <returns> 'True'if the message was processed correctly. </returns>
        public static bool processPayloadReceived(string payload, out string message)
        {
            bool result = false;

            string[] response = payload.Split('|');
            if (response.Length > 1)
            {
                if(response[0] == "1") result = true;
                message = response[1];
            }
            else
            {
                message = "Error: Unexpected message from server";
            }

            return result;
        }

        /// <summary>
        /// Processes user console message to build the payload to send for the server.
        /// </summary>
        /// <param name="userMessage"> User message </param>
        /// <param name="payload"> String payload to be sent to the server. </param>
        /// <returns> 'True'if the payload was generated successfuly. </returns>
        public static bool preparePayloadToSend(string userMessage, out string payload)
        {
            string command = null;
            string arg = null;
            string body = null;
            bool valid = false;

            if (userMessage.StartsWith('/'))
            {
                command = userMessage.Substring(1, 1);

                // private message
                if (command.ToLower() == "p")
                {
                    string aux = userMessage.Substring(userMessage.IndexOf(" ")).Trim();
                    arg = aux.Substring(0, aux.IndexOf(" "));

                    if (arg.Length > 0)
                    {
                        body = aux.Substring(aux.IndexOf(" ")).Trim();
                        valid = true;
                    }
                }
                // logout
                else if (command.ToLower() == "e" && userMessage.ToLower().StartsWith("/exit")) 
                {
                    command = "x";
                    body = userMessage;
                    valid = true;
                }
            }
            else
            {
                command = "n"; //no command. will be ignored at the server
                body = userMessage;                
                valid = true;
            }

            payload = command + (String.IsNullOrEmpty(arg) ? string.Empty : ("#" + arg)) + "|" + body;

            return valid;
        }
    }
}