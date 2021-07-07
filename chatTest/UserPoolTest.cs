using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using chatServer;

namespace chatTest
{
    [TestClass]
    public class UserPoolTest
    {
        [TestMethod]
        public void AddingExistingUserTest()
        {
            TcpClient tcpClient = new TcpClient();
            string user = "Alice";

            UserPool.getInstance().addUser(user, tcpClient);
            bool result =  UserPool.getInstance().addUser(user, tcpClient);

            Assert.IsFalse(result,
                string.Format("Expected not to be allowed users with multiple nicknames. User added twice: {0}", user));
        }

        [TestMethod]
        public void RemovingNonexistentUserTest()
        {
            string user = "Bob";

            bool result =  UserPool.getInstance().removeUser(user);

            Assert.IsFalse(result,
                string.Format("Expected not to be able to remove nonexistent user. User removed without being added: {0}", user));
        }
    }
}
