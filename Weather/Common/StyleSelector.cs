using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Utils;
using Windows.UI.Xaml;

namespace Weather.App.Common
{
    public class StyleSelector
    {
        public static void SetStyle()
        {
            string themesUri = null;
            bool x ;
            string width = "W"+ScreenSizeHelper.WindowSizeWidth();
            ScreenSizeHelper.ScreenWidthEnum screenWidthValue;
            x = Enum.TryParse<ScreenSizeHelper.ScreenWidthEnum>(width, out screenWidthValue);
            if (x)
            {
                switch ((ScreenSizeHelper.ScreenWidthEnum)Enum.Parse(typeof(ScreenSizeHelper.ScreenWidthEnum), width))
                {
                    case ScreenSizeHelper.ScreenWidthEnum.W384:
                        themesUri = "ms-appx:///Themes/W384/DataItemStyle.xaml";
                        break;
                    case ScreenSizeHelper.ScreenWidthEnum.W400:
                        themesUri = "ms-appx:///Themes/W400/DataItemStyle.xaml";
                        break;
                    case ScreenSizeHelper.ScreenWidthEnum.W450:
                        themesUri = "ms-appx:///Themes/W450/DataItemStyle.xaml";
                        break;
                    case ScreenSizeHelper.ScreenWidthEnum.W491:
                        themesUri = "ms-appx:///Themes/W491/DataItemStyle.xaml";
                        break;
                    case ScreenSizeHelper.ScreenWidthEnum.W514:
                        themesUri = "ms-appx:///Themes/W514/DataItemStyle.xaml";
                        break;
                    default:
                        themesUri = "ms-appx:///Themes/W400/DataItemStyle.xaml";
                        break;
                }
            }
            else
            {
                themesUri = "ms-appx:///Themes/W400/DataItemStyle.xaml";
            }
            var appTheme = new ResourceDictionary
            {
                Source = new Uri(themesUri, UriKind.Absolute)
            };
            Application.Current.Resources.MergedDictionaries.Add(appTheme);
        }
    }
}
