using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using CloudyWing.MoneyTrack.Infrastructure.Application;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    /// <summary>
    /// A helper class for working with IP addresses.
    /// </summary>
    [Serializable]
    public class IPAddressContext {
        private readonly UserContext userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPAddressContext"/> class.
        /// </summary>
        /// <param name="userContext">The user context.</param>
        /// <exception cref="System.ArgumentNullException">userContext</exception>
        public IPAddressContext(UserContext userContext) {
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        /// <summary>
        /// Gets the IP address of the client making the request to the server.
        /// </summary>
        public string UserHostAddress => userContext.HttpContext.Request.UserHostAddress;

        /// <summary>
        /// Gets the IP address of the client making the request to the server, giving priority to IPv4 addresses.
        /// </summary>
        public string UserHostAddressPriorityUsingIPv4 =>
            ParsePriorityUsingIPv4(UserHostAddress).ToString();

        /// <summary>
        /// Gets the "real" IP address of the client, taking into account proxies and load balancers.
        /// </summary>
        /// <remarks>
        /// The "real" IP address is determined by looking at the <c>HTTP_X_FORWARDED_FOR</c> server variable. If this variable is present, the first IP address in the comma-separated list is used. Otherwise, <see cref="UserHostAddress"/> is returned.
        /// </remarks>
        public string UserRealAddress {
            get {
                string forwardedFor = userContext.HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                string ip = string.IsNullOrWhiteSpace(forwardedFor)
                    ? UserHostAddress
                    : forwardedFor.Split(',').Select(s => s.Trim()).First();

                return ip == "::1" ? "127.0.0.1" : ip;
            }
        }

        /// <summary>
        /// Gets the formatted IP address of the user.
        /// </summary>
        /// <remarks>
        /// The formatted IP address is a string that represents the IP address in a uniform format.
        /// </remarks>
        public string FormattedIPAddress {
            get {
                string[] ipAddressSegments = UserRealAddress.Split(new char[] { '.' });
                string formattedIPAddress = string.Empty;

                foreach (string ipAddressSegment in ipAddressSegments) {
                    formattedIPAddress += ipAddressSegment.PadLeft(3, '0');
                }

                return formattedIPAddress;
            }
        }

        /// <summary>
        /// Gets the IP address of the client, giving priority to IPv4 addresses.
        /// </summary>
        public string UserRealAddressPriorityUsingIPv4 =>
            ParsePriorityUsingIPv4(UserRealAddress).ToString();

        /// <summary>
        /// Parses the specified IP address string, giving priority to IPv4 addresses.
        /// </summary>
        /// <param name="ip">The IP address string to parse.</param>
        /// <returns>The parsed <see cref="IPAddress"/>.</returns>
        /// <exception cref="FormatException">Thrown when <paramref name="ip"/> is not a valid IP address.</exception>
        public IPAddress ParsePriorityUsingIPv4(string ip) {
            if (TryParsePriorityUsingIPv4(ip, out IPAddress ipAddress)) {
                return ipAddress;
            }

            throw new FormatException("格式錯誤。");
        }

        /// <summary>
        /// Attempts to parse the specified IP address string, giving priority to IPv4 addresses.
        /// </summary>
        /// <param name="ip">The IP address string to parse.</param>
        /// /// <param name="ipAddress">The resulting <see cref="IPAddress"/> object.</param>
        /// <returns><see langword="true"/> if the parsing succeeded, <see langword="false"/> otherwise.</returns>
        public bool TryParsePriorityUsingIPv4(string ip, out IPAddress ipAddress) {
            if (!IPAddress.TryParse(ip, out ipAddress)) {
                ipAddress = null;
                return false;
            }
            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6 && ipAddress.IsIPv4MappedToIPv6) {
                ipAddress = ipAddress.MapToIPv4();
            }

            return true;
        }
    }
}
