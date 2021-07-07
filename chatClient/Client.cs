using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using netLib;

namespace chatClient
{
    public class Client
    {
        /// <summary>
        /// Singleton class instance.
        /// </summary>
        private static Client s_instance = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        Client()
        {   } 

        /// <summary>
        /// Returns singleton instance.
        /// </summary>
        /// <returns> Singleton instance. </returns>
        public static Client getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new Client();
            }
            return s_instance;
        }

        /// <summary>
        /// Runs client chat main communications.
        /// </summary>
        public void run()
        {
            NetworkStream stream = null;
            Listener listener = null;
            string clientMessage;
            bool connected = false;
            TcpClient tcpClient = new TcpClient();
            IPAddress ipAddress = LocalNetInfo.getInstance().getLocalHostIP();

            try
            {
                tcpClient.Connect(ipAddress, 1234); // TODO colocar como parametro    
                connected = true;
            }
            catch (SocketException ex)
            {
                Console.WriteLine(String.Format("It was not possible to connect with the server. Error: {0}", ex.Message));
            }

            if (connected)
            {
                try
                {
                    stream = tcpClient.GetStream();
                    if (this.doHandshake(stream))
                    {
                        listener = this.createBroadcastListener(stream);

                        // sends an ACK to signal the successful handshake
                        MessageBroker.sendMessageToServer(stream, "ACK"); 
                    }

                    // read user chat messages and sends it to the server
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

        /// <summary>
        /// Handles intial handshake with server.
        /// </summary>
        /// <param name="stream"> TCP socket stream for communication. </param>
        /// <returns> Returns 'true' if the handshake was successful. </returns>
        private bool doHandshake(NetworkStream stream)
        {
            bool response = false;
            string clientMessage;
            string serverMessage;

            while (true)
            {
                response = MessageBroker.getServerResponse(stream, out serverMessage);
                Console.WriteLine(serverMessage);
                if (response) break;

                clientMessage = Console.ReadLine();
                MessageBroker.sendMessageToServer(stream, clientMessage);
            }

            return response;
        }

        /// <summary>
        /// Creates thread for listening broadcast communications from server
        /// </summary>
        /// <param name="stream"> TCP socket stream for communication. </param>
        /// <returns> Created listener object. </returns>
        private Listener createBroadcastListener(NetworkStream stream)
        {
            Thread threadListener = null;

            Listener listener = new Listener(stream);
            threadListener = new Thread(new ThreadStart(listener.run));
            threadListener.IsBackground = true;
            threadListener.Start();

            return listener;
        }
    }   
}
