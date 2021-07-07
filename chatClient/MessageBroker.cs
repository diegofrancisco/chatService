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
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}