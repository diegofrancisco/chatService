using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using netLib;

namespace chatServer
{
    public class ServerMgr
    {
        /// <summary>
        /// Singleton class instance.
        /// </summary>
        private static ServerMgr s_instance = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        ServerMgr()
        {   }

                /// <summary>
        /// Returns singleton instance.
        /// </summary>
        /// <returns> Singleton instance. </returns>
        public static ServerMgr getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new ServerMgr();
            }
            return s_instance;
        }

        /// <summary>
        /// Starts the chat server TCP listener
        /// </summary>
        public void run()
        {
            // Gets the local host IP address
            IPAddress ipAddress = LocalNetInfo.getInstance().getLocalHostIP();

            TcpListener tcpListener = new TcpListener(ipAddress, 1234); // TODO colocar como parametro de configuracao
            try
            {
                tcpListener.Start();
            }
            catch (SocketException ex)
            {
                // TODO adicionar lib de logs
                Console.WriteLine("Exception ex", ex.Message);
            }

            while (true)
            {
                ///TODO pensar numa maneira de parar o servidor << usando asyn???
                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                Console.WriteLine("New user connected!!");

                ChatInstance chat = new ChatInstance(string.Empty, tcpClient);
                Thread thread = new Thread(new ThreadStart(chat.run));
                thread.IsBackground = true;
                thread.Start();
            }
        }
    }
}