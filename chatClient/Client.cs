using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using netLib;

namespace chatClient
{
    public static class Client
    {
        public static void run()
        {
            // Gets the local host IP address
            IPAddress ipAddress = LocalNetInfo.getInstance().getLocalHostIP();
            NetworkStream stream = null;

            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(ipAddress, 1234); // TODO colocar como parametro           

            try
            {
                stream = tcpClient.GetStream();
                while (true)
                {
                    byte[] receivedBytes = new byte[1024];
                    int byte_count = stream.Read(receivedBytes, 0, receivedBytes.Length);
                    byte[] formated = new byte[byte_count];
                    //handle  the null characteres in the byte array
                    Array.Copy(receivedBytes, formated, byte_count);
                    string data = Encoding.ASCII.GetString(formated);

                    string[] response = data.Split('|');
                    Console.WriteLine(response[1] + '\n');
                    if(response[0] == "1") break;

                    string message = Console.ReadLine();
                    byte[] buffer = Encoding.ASCII.GetBytes(message);
                    stream.Write(buffer, 0, buffer.Length);
                }

                Listener listener = new Listener(stream);
                Thread thread = new Thread(new ThreadStart(listener.run));
                thread.IsBackground = true;
                thread.Start();

                while (true)
                {
                    string command = Console.ReadLine();
                    byte[] buffer = Encoding.ASCII.GetBytes(command);
                    stream.Write(buffer, 0, buffer.Length);
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
