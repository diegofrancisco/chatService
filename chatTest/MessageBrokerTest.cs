using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using chatServer;

namespace chatTest
{
    [TestClass]
    public class MessageBrokerTest
    {
        [TestMethod]
        public void TestAddingExistingUser()
        {
            TcpClient tcpClient = new TcpClient();
            string user = "Alice";

            MessageBroker.getInstance().addUser(user, tcpClient);
            bool result =  MessageBroker.getInstance().addUser(user, tcpClient);

            Assert.IsFalse(result,
                string.Format("Expected not to be allowed users with multiple nicknames. User added twice: {0}", user));
        }

        [TestMethod]
        public void RemovingNonexistentUser()
        {
            string user = "Bob";

            bool result =  MessageBroker.getInstance().removeUser(user);

            Assert.IsFalse(result,
                string.Format("Expected not to be able to remove nonexistent user. User removed without being added: {0}", user));
        }
    }
}
