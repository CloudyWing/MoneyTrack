using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace System.Web {
    public static class HttpRequestExtensions {
        public static string GetUserHostAddress(this HttpRequest request) {
            return ConvertToIPv4IfIsDefault(request.UserHostAddress);
        }

        private static string ConvertToIPv4IfIsDefault(string ip) {
            return ip == "::1" ? "127.0.0.1" : ip;
        }

        public static string GetUserHostAddress(this HttpRequestBase request) {
            return ConvertToIPv4IfIsDefault(request.UserHostAddress);
        }

        public static string GetUserHostAddressPriorityUsingIPv4(this HttpRequest request) {
            return ParsePriorityUsingIPv4(GetUserHostAddress(request)).ToString();
        }

        public static string GetUserHostAddressPriorityUsingIPv4(this HttpRequestBase request) {
            return ParsePriorityUsingIPv4(GetUserHostAddress(request)).ToString();
        }

        public static string GetUserRealAddress(this HttpRequest request) {
            string forwardedFor = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            string ip = string.IsNullOrWhiteSpace(forwardedFor)
                ? request.UserHostAddress
                : forwardedFor.Split(',').Select(s => s.Trim()).First();

            return ConvertToIPv4IfIsDefault(ip);
        }

        public static string GetUserRealAddress(this HttpRequestBase request) {
            string forwardedFor = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            string ip = string.IsNullOrWhiteSpace(forwardedFor)
                ? request.UserHostAddress
                : forwardedFor.Split(',').Select(s => s.Trim()).First();

            return ConvertToIPv4IfIsDefault(ip);
        }

        public static string GetUserRealAddressPriorityUsingIPv4(this HttpRequest request) {
            return ParsePriorityUsingIPv4(GetUserRealAddress(request)).ToString();
        }

        public static string GetUserRealAddressPriorityUsingIPv4(this HttpRequestBase request) {
            return ParsePriorityUsingIPv4(GetUserRealAddress(request)).ToString();
        }

        private static IPAddress ParsePriorityUsingIPv4(string ip) {
            if (TryParsePriorityUsingIPv4(ip, out IPAddress ipAddress)) {
                return ipAddress;
            }

            throw new FormatException();
        }

        private static bool TryParsePriorityUsingIPv4(string ip, out IPAddress ipAddress) {
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
