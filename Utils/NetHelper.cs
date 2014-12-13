using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Weather.Utils
{
    /// <summary>
    /// 网络相关
    /// </summary>
    public static class NetHelper
    {
        /// <summary>
        /// 网络是否开启
        /// </summary>
        /// <returns></returns>
        public static bool IsNetworkAvailable()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

        }


        /// <summary>
        /// Checks if Internet connection is available. May not be the most best way to do it though
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedToInternet()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();

            return
                (connectionProfile != null &&
                connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }


        /// <summary>
        /// Wifi是否连接
        /// </summary>
        public static bool IsWifiConnection()
        {
            ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

            return internetConnectionProfile.IsWlanConnectionProfile;

        }

        /// <summary>
        /// 移动网络是否连接
        /// </summary>
        /// <returns></returns>
        public static bool IsWwanConnection()
        {
            ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

            return internetConnectionProfile.IsWwanConnectionProfile;
        }

        /// <summary>
        /// 获取手机IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetPhoneIP()
        {
            string ipAddress = null;
            List<string> IPAddress = new List<string>();
            var hostNames = Windows.Networking.Connectivity.NetworkInformation.GetHostNames();
            foreach (var hn in hostNames)
            {
                if (hn.IPInformation != null)
                {
                    ipAddress = hn.DisplayName;
                    IPAddress.Add(ipAddress);
                }
            }
            return IPAddress.FirstOrDefault();
        }
    }
}
