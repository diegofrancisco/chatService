using System.Net;

namespace netLib
{
    public class LocalNetInfo
    {
        /// <summary>
        /// Singleton class instance.
        /// </summary>
        private static LocalNetInfo s_instance = null;

        /// <summary>
        /// Private constructor.
        /// </summary>
        LocalNetInfo()
        {
        }

        /// <summary>
        /// Returns singleton instance.
        /// </summary>
        /// <returns> Singleton instance. </returns>
        public static LocalNetInfo getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new LocalNetInfo();
            }
            return s_instance;
        }

        /// <summary>
        /// Returns the localhost ip address.
        /// </summary>
        /// <returns> IP Address</returns>
        public IPAddress getLocalHostIP()
        {
            IPAddress ipAddress = null;

            ipAddress = IPAddress.Parse("127.0.0.1");

            return ipAddress;
        }
    }
}
