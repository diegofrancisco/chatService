using Microsoft.VisualStudio.TestTools.UnitTesting;
using chatClient;

namespace chatTest
{
    [TestClass]
    public class ClienteMessageBrokerTest
    {
        [TestMethod]
        public void InvalidMessageReceivedTest()
        {
            string message;
            bool result;

            string payload = "NO FLAG PAYLOAD";

            result = MessageBroker.processPayloadReceived(payload, out message);

            Assert.IsFalse(result,
                string.Format("All messages from server are expected to have a flag at the begining. Message without flag was accepted by the client: {0}", payload));
        }

         [TestMethod]
        public void InvalidUserCommandTest()
        {
            string payload;
            bool result;

            string userMessage = "/k user1 Hello World";

            result = MessageBroker.preparePayloadToSend(userMessage, out payload);

            Assert.IsFalse(result,
                string.Format("User commands expected to be validated. Inexistent command accepted by the client: {0}", userMessage));
        }
    }
}