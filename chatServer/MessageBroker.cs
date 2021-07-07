using System;
using System.Net.Sockets;
using System.Text;

namespace chatServer
{
    public class MessageBroker
    {
        /// <summary>
        /// Writes message to client into the data stream.
        /// </summary>
        /// <param name="stream"> TCP socket stream for communication. </param>
        /// <param name="sendMessage"> Body message to be sent. </param>
        /// <param name="command"> Command to be attached to the payload. </param>
        public static void sendMessageToClient(NetworkStream stream, string sendMessage, int command)
        {
            byte[] sendBuffer = new byte[1024]; // TODO put into a constant

            // writes data stream into socket
            sendBuffer = Encoding.ASCII.GetBytes(command.ToString() + "|" + sendMessage);
            stream.Write(sendBuffer, 0, sendBuffer.Length);
        }

        /// <summary>
        /// Reads response from the socket data stream. 
        /// </summary>
        /// <param name="stream"> TCP socket stream for communication. </param>
        /// <param name="command"> Command request by the client. </param>
        /// <returns> Body messsage received. </returns>
        public static string getClientResponse(NetworkStream stream, out string command)
        {
            byte[] receivedBuffer = new byte[1024];
            string message;

            // reads data stream from socket
            int byte_count = stream.Read(receivedBuffer, 0, receivedBuffer.Length);
            byte[] formated = new byte[byte_count];
            Array.Copy(receivedBuffer, formated, byte_count); 
            string data = Encoding.ASCII.GetString(formated);

            if (!MessageBroker.processPayloadReceived(data, out command, out message))
            {
                command = "e";
                message = "Error: Unexpected message from client";
            }

            return message;
        }

        /// <summary>
        /// Process message received from the client.
        /// </summary>
        /// <param name="payload"> String payload received from the client. </param>
        /// <param name="command"> Command request by the client. </param>
        /// <param name="message"> Body messsage received. </param>
        /// <returns> 'True'if the message was processed correctly. </returns>
        public static bool processPayloadReceived(string payload, out string command, out string message)
        {
            bool valid = false;

            command = string.Empty;
            message = string.Empty;

            string[] response = payload.Split('|');
            if (response.Length > 1)
            {
                command = response[0];
                message = response[1];
                valid = true;
            }

            return valid;
        }
    }
}