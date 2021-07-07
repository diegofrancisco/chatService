using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace chatServer
{
    public class UserPool
    {
        /// <summary>
        /// Singleton class instance.
        /// </summary>
        private static UserPool s_instance = null;

        /// <summary>
        /// Lock object to control concorrence
        /// </summary>
        private object m_lock = new Object();

        /// <summary>
        /// Mapping of all users.
        /// </summary>
        private Dictionary<string, TcpClient> m_usersMap = new Dictionary<string, TcpClient>();

        /// <summary>
        /// Constructor.
        /// </summary>
        UserPool()
        { }

        /// <summary>
        /// Returns singleton instance.
        /// </summary>
        /// <returns> Singleton instance. </returns>
        public static UserPool getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new UserPool();
            }
            return s_instance;
        }

        public List<TcpClient> getUsers()
        {
            List<TcpClient> clients = null;

            lock (m_lock)
            {
                clients = new List<TcpClient>(this.m_usersMap.Values);
            }

            return clients;
        }

        public bool addUser(string user, TcpClient tcpClient)
        {
            bool success = false;

            lock (m_lock)
            {
                if (!this.m_usersMap.ContainsKey(user))
                {
                    this.m_usersMap.Add(user, tcpClient);
                    success = true;
                }
            }

            return success;
        }

        public bool removeUser(string user)
        {
            bool success = false;

            lock (m_lock)
            {
                if (this.m_usersMap.ContainsKey(user))
                {
                    this.m_usersMap.Remove(user);
                    success = true;
                }
            }

            return success;
        }

        public async Task broadcastAsync(string message)
        {
            List<TcpClient> users = this.getUsers();

            foreach (TcpClient socket in users)
            {
                NetworkStream stream = socket.GetStream();
                await MessageBroker.sendMessageToClientAsync(stream, message, 1);
            }
        }

        public void broadcast(string message)
        {
            List<TcpClient> users = this.getUsers();

            foreach (TcpClient socket in users)
            {
                NetworkStream stream = socket.GetStream();
                MessageBroker.sendMessageToClient(stream, message, 1);
            }
        }
    }
}