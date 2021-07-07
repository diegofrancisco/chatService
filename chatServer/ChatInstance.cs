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
                 await UserPool.getInstance().broadcastAsync(broadcastMessage);
            });
            Task.WaitAll(t);
        }

        private void broadcastToAllClients(string broadcastMessage) 
        {
            UserPool.getInstance().broadcast(broadcastMessage);
        }

        private void sendDM(string message, string from, string to) 
        {
            UserPool.getInstance().sendDM(message, from, to);
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
            string command;

            MessageBroker.sendMessageToClient(stream, "Welcome to out chat server! Please provide a nickname: ", 0);

            while (true)
            {
                nickname = MessageBroker.getClientResponse(stream, out command);

                if (String.IsNullOrEmpty(nickname))
                {
                    MessageBroker.sendMessageToClient(stream, "Please enter non empty nickname", 0);
                }
                else if (UserPool.getInstance().addUser(nickname.Trim(), this.socket))
                {
                    this.user = nickname.Trim();
                    MessageBroker.sendMessageToClient(stream, String.Format("You are registered as {0}. Joining #general", nickname), 1);
                    MessageBroker.getClientResponse(stream, out command); // Receive ACK before sending broadcast
                    this.broadcastToAllClients(String.Format("{0} has joined #general", nickname));
                    Console.WriteLine(String.Format("User {0} registered and is now online.", nickname));
                    break;
                }
                else
                {
                    MessageBroker.sendMessageToClient(stream, String.Format("Sorry, the nickname {0} is already taken. Please choose a different one: ", nickname), 0);
                }
            }

            while (true)
            {
                if (stream.DataAvailable)
                {
                    string message = MessageBroker.getClientResponse(stream, out command);

                    if (command == "e") // error processing client message
                    {
                        Console.WriteLine(String.Format("Error processing message from {0}", nickname));
                    }
                    else if (command.StartsWith("p")) // private message
                    {
                        string[] aux = command.Split("#");
                        string arg = aux[1];
                        this.sendDM(message, nickname, arg);
                    }
                    else if (command == "x") // user exited
                    {
                        UserPool.getInstance().removeUser(nickname);
                        break;
                    }
                    else
                    {
                        this.broadcastToAllClients(String.Format("{0} says: {1}", nickname, message));
                    }
                }
                Thread.Sleep(500);
            }

            if (stream != null) stream.Close();
            Console.WriteLine(String.Format("User {0} left and is now offline.", nickname));
        }
    }
}