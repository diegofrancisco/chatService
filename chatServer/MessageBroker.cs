using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace chatServer
{
    public class MessageBroker
    {
        public static void sendMessageToClient(NetworkStream stream, string sendMessage, int command)
        {
            byte[] sendBuffer = new byte[1024]; // TODO put into a constant

            sendBuffer = Encoding.ASCII.GetBytes(command.ToString() + "|" + sendMessage);
            stream.Write(sendBuffer, 0, sendBuffer.Length);
        }

        public static async Task sendMessageToClientAsync(NetworkStream stream, string sendMessage, int command)
        {
            byte[] sendBuffer = new byte[1024]; // TODO put into a constant

            sendBuffer = Encoding.ASCII.GetBytes(command + '|' + sendMessage);
            await stream.WriteAsync(sendBuffer,0, sendBuffer.Length);
        }

        public static string getClientResponse(NetworkStream stream, out string command)
        {
            byte[] receivedBuffer = new byte[1024];
            string message;

            int byte_count = stream.Read(receivedBuffer, 0, receivedBuffer.Length);
            byte[] formated = new byte[byte_count];
            Array.Copy(receivedBuffer, formated, byte_count); 
            string data = Encoding.ASCII.GetString(formated);

            string[] response = data.Split('|');
            if (response.Length > 1)
            {
                command = response[0];
                message = response[1];
            }
            else
            {
                command = "e";
                message = "Error: Unexpected message from client";
            }

            return message;
        }
    }
}