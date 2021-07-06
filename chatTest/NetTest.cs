using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Net;
using netLib;

namespace chatTest
{
    [TestClass]
    public class NetTest
    {
        [TestMethod]
        public void TestLocalHostNotNull()
        {
            IPAddress ip = LocalNetInfo.getInstance().getLocalHostIP();
            Assert.IsNotNull(ip, "Expected LocalNetInfo.getInstance().getLocalHostIP() not null");
        }

        [TestMethod]
        public void TestValidIPAddress()
        {
            bool result;
            string pattern = @"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}$";

            IPAddress ip = LocalNetInfo.getInstance().getLocalHostIP();

            // Creates and validates IP Regex  
            Regex rg = new Regex(pattern);
            result = rg.Match(ip.ToString()).Success;

            Assert.IsFalse(!result,
                string.Format("Expected for a valid IP address; Actual {0}", ip.ToString()));
        }
    }
}
