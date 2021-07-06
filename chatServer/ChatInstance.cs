using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chatServer
{
    public class ChatInstance
    {
        public TcpClient socket { get; set; }
        public string user { get; set; }

        private void broadcastToAllClientsAsync(string broadcastMessage) // TODO revisar
        {
            Task t = new Task(async () =>
            {
                 await MessageBroker.getInstance().broadcastAsync(broadcastMessage);
            });
            Task.WaitAll(t);
        }

        private void broadcastToAllClients(string broadcastMessage) 
        {
            MessageBroker.getInstance().broadcast(broadcastMessage);
        }

        private void sendMessageToClient(NetworkStream stream, string sendMessage)
        {
            byte[] sendBuffer = new byte[1024]; // TODO put into a constant

            sendBuffer = Encoding.ASCII.GetBytes(sendMessage);
            stream.Write(sendBuffer, 0, sendBuffer.Length);

        }

        private string getClientResponse(NetworkStream stream)
        {
            byte[] receivedBuffer = new byte[1024];
            string msg;

            int byte_count = stream.Read(receivedBuffer, 0, receivedBuffer.Length);
            byte[] formated = new byte[byte_count];
            Array.Copy(receivedBuffer, formated, byte_count); 
            msg = Encoding.ASCII.GetString(formated);

            return msg;
        }

        public ChatInstance(string user, TcpClient tcpClient)
        {
            this.user = user;
            this.socket = tcpClient;
        }

        public void run()
        {            
            NetworkStream stream = this.socket.GetStream();
            string nickname;

            this.sendMessageToClient(stream, "1|Welcome to out chat server! Please provide a nickname: ");

            while (true)
            {
                nickname = this.getClientResponse(stream);

                if (String.IsNullOrEmpty(nickname))
                {
                    this.sendMessageToClient(stream, "0|Please enter non empty nickname");
                }
                else if (MessageBroker.getInstance().addUser(nickname.Trim(), this.socket))
                {
                    this.user = nickname.Trim();
                    this.sendMessageToClient(stream, String.Format("1|You are registered as {0}. Joining #general", nickname)); // TODO put all strings in a resource
                    this.broadcastToAllClients(String.Format("{0} has joined #general", nickname));
                    break;
                }
                else
                {
                    this.sendMessageToClient(stream, String.Format("0|Sorry, the nickname {0} is already taken. Please choose a different one: ", nickname));
                }
            }

            while (true)
            {
                if (stream.DataAvailable)
                {
                    string command = this.getClientResponse(stream);
                    this.broadcastToAllClients(String.Format("{0} says: {1}", nickname, command));
                }
                Thread.Sleep(500);
            }
        }
    }
}