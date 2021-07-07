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
                            Listener listener = new Listener(stream);
                            Thread thread = new Thread(new ThreadStart(listener.run));
                            thread.IsBackground = true;
                            thread.Start();

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
