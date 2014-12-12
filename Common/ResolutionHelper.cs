using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml;

namespace Weather.Utils
{
    public static class ResolutionHelper
    {
        /// <summary>
        /// 获取分辨率
        /// </summary>
        private static double GetResolutions()
        {
            double percent = 0;
            var scaleFactor = DisplayInformation.GetForCurrentView().ResolutionScale;
            ResolutionScale resolutionScale = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().ResolutionScale;
            switch (resolutionScale)
            {
                case ResolutionScale.Invalid:
                    break;
                case ResolutionScale.Scale100Percent:
                    percent = 1;
                    break;
                case ResolutionScale.Scale120Percent:
                    percent = 1.2;
                    break;
                case ResolutionScale.Scale140Percent:
                    percent = 1.4;
                    break;
                case ResolutionScale.Scale150Percent:
                    percent = 1.5;
                    break;
                case ResolutionScale.Scale160Percent:
                    percent = 1.6;
                    break;
                case ResolutionScale.Scale180Percent:
                    percent = 1.8;
                    break;
                case ResolutionScale.Scale225Percent:
                    percent = 2.25;
                    break;
                default:
                    break;
            }
            return percent;
        }

        public static double GetResolutionsForWidth()
        {
            double result = Window.Current.Bounds.Width * GetResolutions();
            return result;
        }

        public static double GetResolutionsForHeight()
        {
            double result = Window.Current.Bounds.Height * GetResolutions();
            return result;
        }
    }
}
