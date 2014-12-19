using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Utils
{
    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    public static class MessageHelper
    {
        /// <summary>
        /// Used to display messages to the user
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        //public void NotifyUser(string strMessage, NotifyType type)
        //{
        //    if (StatusBlock != null)
        //    {
        //        switch (type)
        //        {
        //            case NotifyType.StatusMessage:
        //                StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
        //                break;
        //            case NotifyType.ErrorMessage:
        //                StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
        //                break;
        //        }
        //        StatusBlock.Text = strMessage;

        //        // Collapse the StatusBlock if it has no text to conserve real estate.
        //        if (StatusBlock.Text != String.Empty)
        //        {
        //            StatusBorder.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //        }
        //        else
        //        {
        //            StatusBorder.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //        }
        //    }
        //}
    }
}
