using Microsoft.VisualStudio.TestTools.UnitTesting;
using chatServer;

namespace chatTest
{
    [TestClass]
    public class ServerMessageBrokerTest
    {
        [TestMethod]
        public void InvalidMessageReceivedTest()
        {
            string command;
            string message;
            bool result;

            string payload = "NO COMMAND PAYLOAD";

            result = MessageBroker.processPayloadReceived(payload, out command, out message);

            Assert.IsFalse(result,
                string.Format("All messages from client are expected to have a command at the begining. Message without command was accepted by the server: {0}", payload));
        }
    }
}