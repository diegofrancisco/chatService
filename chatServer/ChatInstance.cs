using System;
using System.Net.Sockets;
using System.Threading;

namespace chatServer
{
    public class ChatInstance
    {
        /// <summary>
        /// TCP socket stream for communication.
        /// </summary>
        public TcpClient socket { get; set; }

        /// <summary>
        /// user nickname.
        /// </summary>
        public string userNickname { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user"> user nickname. </param>
        /// <param name="tcpClient"> TCP socket stream for communication. </param>
        public ChatInstance(string user, TcpClient tcpClient)
        {
            this.userNickname = user;
            this.socket = tcpClient;
        }

        /// <summary>
        /// Starts chat instance for a given user logged in the server.
        /// </summary>
        public void run()
        {            
            NetworkStream stream = this.socket.GetStream();
            string command;

            if (this.doHandshake(stream))
            {
                // process user chat commands/messages
                while (true)
                {
                    if (stream.DataAvailable)
                    {
                        string message = MessageBroker.getClientResponse(stream, out command);

                        if (command == "e") // error processing client message
                        {
                            Console.WriteLine(String.Format("Error processing message from {0}", userNickname));
                        }
                        else if (command.StartsWith("p")) // private message
                        {
                            string[] aux = command.Split("#");
                            string arg = aux[1];
                            UserPool.getInstance().sendDM(message, userNickname, arg);
                        }
                        else if (command == "x") // user exited
                        {
                            UserPool.getInstance().removeUser(userNickname);
                            break;
                        }
                        else // standard chat message to be broadcast to all logged users
                        {
                            UserPool.getInstance().broadcast(String.Format("{0} says: {1}", userNickname, message));
                        }
                    }
                    Thread.Sleep(500);
                }
            }

            if (stream != null) stream.Close();
            Console.WriteLine(String.Format("User {0} left and is now offline.", userNickname));
        }

        /// <summary>
        /// Handles initial handshake with the client.
        /// </summary>
        /// <param name="stream"> TCP socket stream for communication. </param>
        /// <returns> Returns 'true' if the handshake was successful. </returns>
        private bool doHandshake(NetworkStream stream)
        {
            string nickname;
            string command;
            bool success = false;

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
                    this.userNickname = nickname.Trim();
                    MessageBroker.sendMessageToClient(stream, String.Format("You are registered as {0}. Joining #general", nickname), 1);
                    MessageBroker.getClientResponse(stream, out command); // Receive ACK before sending broadcast                    
                    UserPool.getInstance().broadcast(String.Format("{0} has joined #general", nickname));
                    Console.WriteLine(String.Format("User {0} registered and is now online.", nickname));
                    success = true;
                    break;
                }
                else
                {
                    MessageBroker.sendMessageToClient(stream, String.Format("Sorry, the nickname {0} is already taken. Please choose a different one: ", nickname), 0);
                }
            }

            return success;
        }
    }
}