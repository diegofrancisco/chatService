using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace chatClient
{
    public class Listener
    {
        private NetworkStream mStream = null;
        public Listener(NetworkStream stream)
        {
            this.mStream = stream;
        }

        public void run()
        {
            while (true)
            {
                if (this.mStream.DataAvailable)
                {
                    byte[] receivedBytes = new byte[1024];
                    int byte_count = this.mStream.Read(receivedBytes, 0, receivedBytes.Length);
                    byte[] formated = new byte[byte_count];
                    //handle  the null characteres in the byte array
                    Array.Copy(receivedBytes, formated, byte_count);
                    string data = Encoding.ASCII.GetString(formated);

                    Console.WriteLine(data + '\n');
                }else
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}