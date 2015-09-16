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
using Weather.Utils;
using Weather.Service.Message;
using Weather.Service.Implementations;
using Windows.Phone.UI.Input;
using System.Threading.Tasks;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Weather.App
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private SettingService settingService = null;
        private UserService userService = null;
        private GetSettingSwitchesRespose switchesRespose = null;
        private GetSettingAutoUpdateTimeRepose autoUpdateTimeRepose = null;
        private GetUserRespose userRespose = null;

        public SettingPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            settingService = SettingService.GetInstance();
            userService = UserService.GetInstance();
            switchesRespose = new GetSettingSwitchesRespose();
            autoUpdateTimeRepose = new GetSettingAutoUpdateTimeRepose();
            userRespose = new GetUserRespose();

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

            await LoadData();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async Task LoadData()
        {
            switchesRespose = settingService.GetSettingSwitches();
            autoUpdateTimeRepose = settingService.GetSettingAutoUpdateTime();
            userRespose = await userService.GetUserAsync();

            ViewModel.AutoUpdateSettingPage autoUpdateSettingPage = new ViewModel.AutoUpdateSettingPage()
            {
                Switches = switchesRespose.Switches,
                AutoUpdateTimes = autoUpdateTimeRepose.AutoUpdateTimes
            };
            ViewModel.GeneralSettingPage generalSettingPage = new ViewModel.GeneralSettingPage()
            {
                Switches = switchesRespose.Switches
            };
            ViewModel.SettingPage settingPage = new ViewModel.SettingPage()
            {
                AutoUpdateSettingPage = autoUpdateSettingPage,
                GeneralSettingPage = generalSettingPage,
                UserConfig = userRespose.UserConfig

            };
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = settingPage;
        }

        private void NotifyUser(string strMessage)
        {
            StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);

            StatusBlock.Text = strMessage;

            if (StatusBlock.Text != String.Empty)
            {
                popup.IsOpen = true;
                // 创建一个DispatcherTimer实例。
                DispatcherTimer newTimer = new DispatcherTimer();
                // 将DispatcherTimer的Interval设为3秒。
                newTimer.Interval = TimeSpan.FromSeconds(3);
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

        private void LockScreen_Click(object sender, RoutedEventArgs e)
        {
            SettingPageHelper.LaunchUriAsync(SettingPageHelper.LaunchUriType.Locks);
        }




        /// <summary>
        /// 仅在WI-FI建立后更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbbIsWifiUpdate_DropDownClosed(object sender, object e)
        {
            userRespose.UserConfig.IsWifiUpdate = int.Parse(cbbIsWifiUpdate.SelectedValue.ToString());

            await userService.SaveUser(userRespose);

        }
        /// <summary>
        /// 仅更新默认城市
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbbIsUpdateForCity_DropDownClosed(object sender, object e)
        {
            userRespose.UserConfig.IsUpdateForCity = int.Parse(cbbIsUpdateForCity.SelectedValue.ToString());
            await userService.SaveUser(userRespose);
        }
        /// <summary>
        /// 仅在WI-FI建立后自动更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbbIsWifiAutoUpdate_DropDownClosed(object sender, object e)
        {
            userRespose.UserConfig.IsWifiAutoUpdate = int.Parse(cbbIsWifiAutoUpdate.SelectedValue.ToString());

            await userService.SaveUser(userRespose);
        }

        /// <summary>
        /// 是否允许自动更新默认城市
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbbIsAutoUpdateForCity_DropDownClosed(object sender, object e)
        {
            userRespose.UserConfig.IsAutoUpdateForCity = int.Parse(cbbIsAutoUpdateForCity.SelectedValue.ToString());

            await userService.SaveUser(userRespose);
        }

        /// <summary>
        /// 是否允许自动更新所有常用城市
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbbIsAutoUpdateForCities_DropDownClosed(object sender, object e)
        {
            userRespose.UserConfig.IsAutoUpdateForCities = int.Parse(cbbIsAutoUpdateForCities.SelectedValue.ToString());
            await userService.SaveUser(userRespose);
        }

        /// <summary>
        /// 自动更新频率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbbAutoUpdateTime_DropDownClosed(object sender, object e)
        {
            userRespose.UserConfig.AutoUpdateTime = int.Parse(cbbAutoUpdateTime.SelectedValue.ToString());
            await userService.SaveUser(userRespose);
        }

    }
}
