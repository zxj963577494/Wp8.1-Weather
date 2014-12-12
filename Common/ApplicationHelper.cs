using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Weather.Utils
{
    public static class ApplicationHelper
    {
        public static void Exit()
        {
            Application.Current.Exit();
            //            private async void btnMarketplaceDetail_Click(object sender, RoutedEventArgs e) {
            //                // 打开指定 app 在商店中的详细页 bool success = await Launcher.LaunchUriAsync(new Uri("zune:navigate?appid=dcdd7004-064a-4c04-ad22-eae725f8ffb1")); } 
            //private async void btnMarketplaceReview_Click(object sender, RoutedEventArgs e) { // 在商店中评论指定的 app // bool success = await Launcher.LaunchUriAsync(new Uri("zune:reviewapp?appid=dcdd7004-064a-4c04-ad22-eae725f8ffb1")); await Windows.System.Launcher.LaunchUriAsync(new Uri("zune:reviewapp?appid=" + CurrentApp.AppId)); } 
            //private async void btnMarketplaceSearch_Click(object sender, RoutedEventArgs e) { // 在商店中搜索 app（支持按关键字和发行商搜索） bool success = await Launcher.LaunchUriAsync(new Uri("zune:search?keyword=&amp;publisher=Czh1994&amp;contenttype=app")); }

        }
    }
}
