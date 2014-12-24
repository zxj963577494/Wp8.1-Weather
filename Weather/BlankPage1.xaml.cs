using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Weather.App
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Window Size
            var bounds = Window.Current.Bounds;
            WindowSize.Text = string.Format("H {0}  x  W {1}", bounds.Height, bounds.Width);

            // Logical Dpi
            var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            LogicalDpi.Text = logicalDpi.ToString();

            // RawPixelsPerViewPixel
            var dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            RawPixelsPerViewPixel.Text = dpiRatio.ToString();

            // ScaleFactor
            var scaleFactor = bounds.Width * dpiRatio / 480;
            ScaleFactor.Text = scaleFactor.ToString();

            // ScreenResolution
            var resolutionH = Math.Round(bounds.Height * dpiRatio);
            var resolutionW = Math.Round(bounds.Width * dpiRatio);
            ScreenResolution.Text = string.Format("{0} x {1}", resolutionH, resolutionW);


            // LogicalResolution
            LogicalResolution.Text = string.Format("{0} x {1}", resolutionH / scaleFactor, resolutionW / scaleFactor);

            // RawDpi
            var rawDpiX = DisplayInformation.GetForCurrentView().RawDpiX;
            var rawDpiY = DisplayInformation.GetForCurrentView().RawDpiY;
            RawDpi.Text = string.Format("RawDpiX:{0}， RawDpiY:{1}", rawDpiX, rawDpiY);

            // ScreenInch
            var screenInch = Math.Sqrt(Math.Pow(resolutionH / rawDpiY, 2) + Math.Pow(resolutionW / rawDpiX, 2));
            ScreenInch.Text = string.Format("{0} inches", screenInch.ToString());
        }
    }
}
