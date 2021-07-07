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

        public static string getClientResponse(NetworkStream stream)
        {
            byte[] receivedBuffer = new byte[1024];
            string msg;

            int byte_count = stream.Read(receivedBuffer, 0, receivedBuffer.Length);
            byte[] formated = new byte[byte_count];
            Array.Copy(receivedBuffer, formated, byte_count); 
            msg = Encoding.ASCII.GetString(formated);

            return msg;
        }
    }
}