using System;
using System.Net.Sockets;
using System.Threading;

namespace chatClient
{
    public class Listener
    {
        /// <summary>
        /// TCP socket stream for communication.
        /// </summary>
        private NetworkStream mStream = null;

        /// <summary>
        /// Flag that control that the listener should stay listening to broadcasts from server.
        /// </summary>
        private bool stayAlive = true;
        public bool StayAlive
        {
            set { stayAlive = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream"> TCP socket stream for communication. </param>
        public Listener(NetworkStream stream)
        {
            this.mStream = stream;
        }

        /// <summary>
        /// Runs listener.
        /// </summary>
        public void run()
        {
            string message = null;

            // keeps listening to socket waiting for messages from server
            while (true)
            {
                if (this.stayAlive)
                {
                    if (this.mStream.DataAvailable)
                    {
                        MessageBroker.getServerResponse(this.mStream, out message);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
                else break;
            }
        }
    }
}