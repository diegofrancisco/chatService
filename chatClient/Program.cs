using System;

namespace chatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.startChatClient();
        }

         /// <summary>
        /// Starts chat client.
        /// </summary>
        private static void startChatClient(){
            Client.run();
        }
    }
}
