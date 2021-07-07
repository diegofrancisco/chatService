using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace chatClient
{
    public class Listener
    {
        private NetworkStream mStream = null;
        private bool stayAlive = true;

        public bool StayAlive
        {
            set { stayAlive = value; }
        }

        public Listener(NetworkStream stream)
        {
            this.mStream = stream;
        }

        public void run()
        {
            string message;

            while (true)
            {
                if (this.stayAlive)
                {
                    if (this.mStream.DataAvailable)
                    {
                        MessageBroker.getServerResponse(this.mStream, out message);
                        ConsoleSync.writeToConsoleSync(message);
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