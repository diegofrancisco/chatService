using System;

namespace chatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.startSyncServer();
        }

        /// <summary>
        /// Starts chat server.
        /// </summary>
        private static void startSyncServer(){
            Server.getInstance().run();
        }
    }
}
