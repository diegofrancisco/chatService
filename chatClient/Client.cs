using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using netLib;

namespace chatClient
{
    public class Client
    {
        public Client()
        {   } 

        public void run()
        {
            // Gets the local host IP address
            IPAddress ipAddress = LocalNetInfo.getInstance().getLocalHostIP();
            NetworkStream stream = null;
            Thread threadListener = null;
            Listener listener = null;
            string clientMessage;
            string serverMessage;
            bool connected = false;

            TcpClient tcpClient = new TcpClient();

            try
            {
                tcpClient.Connect(ipAddress, 1234); // TODO colocar como parametro    
                connected = true;
            }
            catch (SocketException ex)
            {
                ConsoleSync.writeToConsoleSync(String.Format("It was not possible to connect with the server. Error: {0}", ex.Message));
            }

            if (connected)
            {
                try
                {
                    stream = tcpClient.GetStream();
                    while (true)
                    {
                        bool response = MessageBroker.getServerResponse(stream, out serverMessage);
                        ConsoleSync.writeToConsoleSync(serverMessage);
                        if (response)
                        {
                            listener = new Listener(stream);
                            threadListener = new Thread(new ThreadStart(listener.run));
                            threadListener.IsBackground = true;
                            threadListener.Start();

                            MessageBroker.sendMessageToServer(stream, "ACK");
                            break;
                        }

                        clientMessage = Console.ReadLine();
                        MessageBroker.sendMessageToServer(stream, clientMessage);
                    }

                    while (true)
                    {
                        clientMessage = Console.ReadLine();
                        MessageBroker.sendMessageToServer(stream, clientMessage);

                        if (clientMessage.Trim().ToLower() == "/exit")
                        {
                            listener.StayAlive = false;
                            break;
                        }
                    }
                }
                finally
                {
                    if (stream != null) stream.Close();
                    if (tcpClient != null) tcpClient.Close();

                    Console.WriteLine("Disconected. Bye!");
                    Console.ReadKey();
                }
            }
        }
    }   
}
