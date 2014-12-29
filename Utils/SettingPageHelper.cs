using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.ApplicationModel.Store;

namespace Weather.Utils
{
    public static class SettingPageHelper
    {
        public async static void LaunchUriAsync(LaunchUriType type)
        {
            switch (type)
            {
                case LaunchUriType.Airplanemode:
                    await Launcher.LaunchUriAsync(new Uri("ms-settings-airplanemode:"));
                    break;
                case LaunchUriType.Bluetooth:
                    await Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
                    break;
                case LaunchUriType.Cellular:
                    await Launcher.LaunchUriAsync(new Uri("ms-settings-cellular:"));
                    break;
                case LaunchUriType.Wifi:
                    await Launcher.LaunchUriAsync(new Uri("ms-settings-wifi:"));
                    break;
                case LaunchUriType.Location:
                    await Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
                    break;
                case LaunchUriType.Emailandaccounts:
                    await Launcher.LaunchUriAsync(new Uri("ms-settings-emailandaccounts:"));
                    break;
                case LaunchUriType.Locks:
                    await Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
                    break;
                case LaunchUriType.Screenrotation:
                    await Launcher.LaunchUriAsync(new Uri("ms-settings-screenrotation:"));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 拨打电话
        /// </summary>
        /// <param name="tel"></param>
        public async static void LaunchUriForTelAsync(string tel)
        {
            string telphone = "tel:" + tel;
            await Launcher.LaunchUriAsync(new Uri(telphone));
            //bool success = await Launcher.LaunchUriAsync(new Uri("callto:1391234567")); } 
        }

        /// <summary>
        ///  打开链接
        /// </summary>
        /// <param name="http"></param>
        public async static void LaunchUriForhttpAsync(string http)
        {
            await Launcher.LaunchUriAsync(new Uri(http));
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mail"></param>
        public async static void LaunchUriForEmailAsync(string mail)
        {
            string mailto = "mailto:" + mail;
            await Launcher.LaunchUriAsync(new Uri(mailto));
        }

        public async static void LaunchUriForMarketplaceDetail()
        {
            await Launcher.LaunchUriAsync(new Uri("zune:navigate?appid=" + CurrentApp.AppId));
        }

        public enum LaunchUriType
        {
            /// <summary>
            /// 飞行模式设置
            /// </summary>
            Airplanemode = 1,
            /// <summary>
            /// 蓝牙设置
            /// </summary>
            Bluetooth = 2,
            /// <summary>
            /// 手机网络设置
            /// </summary>
            Cellular = 3,
            /// <summary>
            /// WiFi设置
            /// </summary>
            Wifi = 4,
            /// <summary>
            /// 定位设置
            /// </summary>
            Location = 5,
            /// <summary>
            /// 电子邮件+账户设置
            /// </summary>
            Emailandaccounts = 6,
            /// <summary>
            /// 锁屏设置
            /// </summary>
            Locks = 7,
            /// <summary>
            /// 屏幕旋转
            /// </summary>
            Screenrotation = 8
        }
    }

   

}
