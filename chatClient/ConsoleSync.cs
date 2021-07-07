using System;

namespace chatClient
{
    public static class ConsoleSync
    {
        /// <summary>
        /// Lock object to control concorrence
        /// </summary>
        private static object s_lock = new Object();

        public static void writeToConsoleSync(string message)
        {
            lock (s_lock)
            {
                Console.WriteLine(message);
            }
        }
    }
}