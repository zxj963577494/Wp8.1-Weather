using Weather.App.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Weather.Service.Implementations;
using Weather.Service.Message;
using System.Threading.Tasks;

using Weather.App;
using Windows.Phone.UI.Input;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Weather.App
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MyCityPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly UserService userService = null;
        private GetUserCityRespose respose = null;

        public MyCityPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件

            userService = new UserService();
        }

        /// <summary>
        /// 获取与此 <see cref="Page"/> 关联的 <see cref="NavigationHelper"/>。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// 获取此 <see cref="Page"/> 的视图模型。
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// 使用在导航过程中传递的内容填充页。  在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        /// 事件的来源; 通常为 <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">事件数据，其中既提供在最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的导航参数，又提供
        /// 此页在以前会话期间保留的状态的
        /// 字典。 首次访问页面时，该状态将为 null。</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {


        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper"/></param>
        ///<param name="e">提供要使用可序列化状态填充的空字典
        ///的事件数据。</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper 注册

        /// <summary>
        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// <para>
        /// 应将页面特有的逻辑放入用于
        /// <see cref="NavigationHelper.LoadState"/>
        /// 和 <see cref="NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。
        /// </para>
        /// </summary>
        /// <param name="e">提供导航方法数据和
        /// 无法取消导航请求的事件处理程序。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            respose = await GetUserCity();
            LayoutRoot.DataContext = respose;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }
        }
        #endregion




        private void abbAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddCityPage));
        }

        private void Grid_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);

                flyoutBase.ShowAt(senderElement);
            }
        }

        private async void DefaultCity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuFlyoutItem selectedItem = sender as MenuFlyoutItem;
                if (selectedItem != null)
                {
                    int cityId = int.Parse(selectedItem.CommandParameter.ToString());
                    respose = await GetUserCity();
                    var DefaultCityed = respose.UserCities.FirstOrDefault(x => x.IsDefault == 1);
                    if (DefaultCityed.CityId != cityId)
                    {
                        respose.UserCities.FirstOrDefault(x => x.CityId == DefaultCityed.CityId).IsDefault = 0;
                        respose.UserCities.FirstOrDefault(x => x.CityId == cityId).IsDefault = 1;
                        userService.SaveUserCity(respose);
                        NotifyUser("设置成功");
                        LayoutRoot.DataContext = SortUserCity(respose);
                    }
                    else
                    {
                        NotifyUser("该城市已是默认城市");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void DesTopTile_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem selectedItem = sender as MenuFlyoutItem;
            if (selectedItem != null)
            {
                int cityId = int.Parse(selectedItem.CommandParameter.ToString());
                string titleId = selectedItem.CommandParameter.ToString() + "_Weather";
                Model.UserCity userCity = (from u in respose.UserCities
                                           where u.CityId == cityId
                                           select u).FirstOrDefault();
                string displayName = userCity.CityName;
                if (!Utils.SecondaryTileHelper.IsExists(titleId))
                {
                    Utils.SecondaryTileHelper.CreateSecondaryTileAsync(titleId, displayName, cityId.ToString());
                }
                else
                {
                    NotifyUser("该城市磁贴已固定在桌面");
                }
            }
        }

        private async Task<GetUserCityRespose> GetUserCity()
        {
            GetUserCityRespose respose = new GetUserCityRespose();
            respose = await userService.GetUserCityAsync();
            return SortUserCity(respose);
        }


        private GetUserCityRespose SortUserCity(GetUserCityRespose respose)
        {
            var data = from c in respose.UserCities
                       orderby c.IsDefault descending, c.AddTime descending
                       select c;
            respose.UserCities = data.ToList();
            return respose;
        }

        /// <summary>
        /// Used to display messages to the user
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        private void NotifyUser(string strMessage)
        {
            StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);

            StatusBlock.Text = strMessage;

            if (StatusBlock.Text != String.Empty)
            {
                popup.IsOpen = true;
                // 创建一个DispatcherTimer实例。
                DispatcherTimer newTimer = new DispatcherTimer();
                // 将DispatcherTimer的Interval设为5秒。
                newTimer.Interval = TimeSpan.FromSeconds(5);
                // 这样一来OnTimerTick方法每秒都会被调用一次。
                newTimer.Tick += (o, e) =>
                {
                    popup.IsOpen = false;
                };
                // 开始计时。
                newTimer.Start();
            }
            else
            {
                popup.IsOpen = false;
            }
        }

        private void RemoveCity_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem selectedItem = sender as MenuFlyoutItem;
            if (selectedItem != null)
            {
                int cityId = int.Parse(selectedItem.CommandParameter.ToString());
                GetUserCityRespose list = SortUserCity(respose);
                Model.UserCity city = list.UserCities.FirstOrDefault(x => x.CityId == cityId);
                if (list.UserCities.FirstOrDefault().IsDefault == 1)
                {
                    list.UserCities.Remove(city);
                    list.UserCities.FirstOrDefault().IsDefault = 1;
                }
                else
                {
                    list.UserCities.Remove(city);
                }
                LayoutRoot.DataContext = null;
                LayoutRoot.DataContext = list;
                userService.SaveUserCity(list);
            }
        }
    }
}
