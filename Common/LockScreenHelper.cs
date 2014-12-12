using NotificationsExtensions.BadgeContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace Weather.Utils
{
    /// <summary>
    /// 锁屏
    /// </summary>
    public static class LockScreenHelper
    {
        /// <summary>
        /// 申请锁屏
        /// </summary>
        public static async void RequestLockScreenAccess()
        {
            BackgroundAccessStatus status = BackgroundAccessStatus.Unspecified;
            try
            {
                status = await BackgroundExecutionManager.RequestAccessAsync();
            }
            catch (UnauthorizedAccessException)
            {
                // An access denied exception may be thrown if two requests are issued at the same time
                // For this specific sample, that could be if the user double clicks "Request access"
            }

            switch (status)
            {
                case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                    break;
                case BackgroundAccessStatus.Denied:
                    break;
                case BackgroundAccessStatus.Unspecified:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        ///移除锁屏
        /// </summary>
        public static void RemoveLockScreenAccess()
        {
            BackgroundExecutionManager.RemoveAccess();
        }

        /// <summary>
        /// 查询锁屏
        /// </summary>
        public static void QueryLockScreenAccess()
        {
            switch (BackgroundExecutionManager.GetAccessStatus())
            {
               
                case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                    break;
                case BackgroundAccessStatus.Denied:
                    break;
                case BackgroundAccessStatus.Unspecified:
                    break;
                default:
                    break;
            }
        }
    }
}
