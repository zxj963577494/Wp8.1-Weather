using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml;

namespace Weather.Utils
{
    /// <summary>
    /// 屏幕尺寸
    /// </summary>
    public class ScreenSizeHelper
    {

        /// <summary>
        /// 表示窗口大小或视图大小，宽度
        /// </summary>
        /// <returns></returns>
        public static string WindowSizeWidth()
        {
            var bounds = Window.Current.Bounds;
            return Math.Round(bounds.Width).ToString();
        }

        /// <summary>
        /// 表示窗口大小或视图大小，高度
        /// </summary>
        /// <returns></returns>
        public static string WindowSizeHeight()
        {
            var bounds = Window.Current.Bounds;
            return Math.Round(bounds.Height).ToString();
        }

        /// <summary>
        /// 表示当前设备每逻辑像素所包含的像素数量
        /// </summary>
        /// <returns></returns>
        public static string LogicalDpi()
        {
            return DisplayInformation.GetForCurrentView().LogicalDpi.ToString();
        }

        /// <summary>
        /// 表示每个可视像素对应的实际像素个数(WP8.1独有)
        /// </summary>
        /// <returns></returns>
        public static string RawPixelsPerViewPixel()
        {
#if WINDOWS_PHONE_APP
             return DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel.ToString();
#else
            return "";
#endif
        }

        /// <summary>
        /// 表示基准分辨率到实际分辨率的扩展因子
        /// </summary>
        /// <returns></returns>
        public static string ScaleFactor()
        {
#if WINDOWS_PHONE_APP
            var bounds = Window.Current.Bounds;
            var dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var scaleFactor = bounds.Width * dpiRatio / 480;
            return scaleFactor.ToString();
#else
            return "";
#endif
        }


        /// <summary>
        /// 表示屏幕分辨率，即实际分辨率，宽度
        /// </summary>
        /// <returns></returns>
        public static string ScreenResolutionWidth()
        {
#if WINDOWS_PHONE_APP
            var bounds = Window.Current.Bounds;
            var dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var resolutionH = Math.Round(bounds.Height * dpiRatio);
            var resolutionW = Math.Round(bounds.Width * dpiRatio);
            return resolutionW.ToString();
#else
            return "";
#endif
        }

        /// <summary>
        /// 表示屏幕分辨率，即实际分辨率，高度
        /// </summary>
        /// <returns></returns>
        public static string ScreenResolutionHeight()
        {
#if WINDOWS_PHONE_APP
            var bounds = Window.Current.Bounds;
            var dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var resolutionH = Math.Round(bounds.Height * dpiRatio);
            var resolutionW = Math.Round(bounds.Width * dpiRatio);
            return resolutionH.ToString();
#else
            return "";
#endif
        }

        /// <summary>
        /// 表示逻辑分辨率，即基准分辨率，宽度
        /// </summary>
        /// <returns></returns>
        public static string LogicalResolutionWidth()
        {
#if WINDOWS_PHONE_APP
            var bounds = Window.Current.Bounds;
            var dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var resolutionW = Math.Round(bounds.Width * dpiRatio);
            var scaleFactor = bounds.Width * dpiRatio / 480;
            return (resolutionW / scaleFactor).ToString();
#else
            return "";
#endif
        }

        /// <summary>
        /// 表示逻辑分辨率，即基准分辨率，高度
        /// </summary>
        /// <returns></returns>
        public static string LogicalResolutionHeight()
        {
#if WINDOWS_PHONE_APP
            var bounds = Window.Current.Bounds;
            var dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var resolutionH = Math.Round(bounds.Height * dpiRatio);
            var scaleFactor = bounds.Width * dpiRatio / 480;
            return (resolutionH / scaleFactor).ToString();
#else
            return "";
#endif
        }

        /// <summary>
        /// 表示屏幕尺寸，即实际屏幕对角线的长度，单位英寸
        /// </summary>
        /// <returns></returns>
        public static string ScreenSize()
        {
#if WINDOWS_PHONE_APP
            var bounds = Window.Current.Bounds;
            var dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var resolutionW = Math.Round(bounds.Width * dpiRatio);
            var resolutionH = Math.Round(bounds.Height * dpiRatio);
            var rawDpiX = DisplayInformation.GetForCurrentView().RawDpiX;
            var rawDpiY = DisplayInformation.GetForCurrentView().RawDpiY;
            var screenInch = Math.Sqrt(Math.Pow(resolutionH / rawDpiY, 2) + Math.Pow(resolutionW / rawDpiX, 2));
            return screenInch.ToString("0.0");
#else
            return "";
#endif
        }

        public enum ScreenSizeEnum
        {
            /// <summary>
            /// 
            /// </summary>
            WVGA = 1,
            /// <summary>
            /// 
            /// </summary>
            WXGA = 2,
            /// <summary>
            /// 
            /// </summary>
            W720P = 3,
            /// <summary>
            /// 
            /// </summary>
            W1080P = 4
        }

        public enum ScreenWidthEnum
        {
            /// <summary>
            /// 4.5inch 1280*768 2
            /// </summary>
            W384 = 1,
            /// <summary>
            /// 4inch 800*400 1 || 4.7inch 1280*720 1.8
            /// </summary>
            W400 = 2,
            /// <summary>
            /// 5inch 540*960 1.2 || 5.5inch 1080*1920 2.4 || 5inch 720*1280 1.6
            /// </summary>
            W450 = 3,
            /// <summary>
            /// 6inch 1080*1920 2.2 ||
            /// </summary>
            W491 = 4,
            /// <summary>
            /// 6inch 720*1280 1.4
            /// </summary>
            W514 = 5
        }

    }

}
