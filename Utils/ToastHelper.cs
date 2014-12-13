using NotificationsExtensions.ToastContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Weather.Utils
{
    /// <summary>
    /// 通知类
    /// </summary>
    public class ToastHelper
    {
        /// <summary>
        /// 创建通知信息
        /// </summary>
        /// <param name="toastText"></param>
        public static void CreateToast(string toastText)
        {
            IToastNotificationContent toastContent = null;

            IToastText01 templateContent = ToastContentFactory.CreateToastText01();

            templateContent.TextBodyWrap.Text = toastText;
            templateContent.Audio.Content = ToastAudioContent.SMS;
           

            toastContent = templateContent;

            // Create a toast from the Xml, then create a ToastNotifier object to show
            // the toast
            ToastNotification toast = toastContent.CreateNotification();

            // If you have other applications in your package, you can specify the AppId of
            // the app to create a ToastNotifier for that application
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }



}
