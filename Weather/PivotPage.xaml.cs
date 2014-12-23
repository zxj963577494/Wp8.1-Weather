using Weather.App.Common;
using Weather.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
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

using Weather.Utils;
using Windows.ApplicationModel.Background;
using Weather.Service.Message;
using Weather.Service.Implementations;
using Windows.Storage;

// “透视应用程序”模板在 http://go.microsoft.com/fwlink/?LinkID=391641 上有介绍

namespace Weather.App
{
    public sealed partial class PivotPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        private UserService userService;
        private WeatherService weatherService;
        private GetUserRespose userRespose;
        private GetUserCityRespose userCityRespose;
        private GetWeatherRespose weatherRespose;
        private GetWeatherTypeRespose weatherTypeRespose;
        private string cityId = null;

        long LastExitAttemptTick = DateTime.Now.Ticks;

        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;


            userService = new UserService();
            weatherService = new WeatherService();
            userRespose = new GetUserRespose();
            userCityRespose = new GetUserCityRespose();
            weatherRespose = new GetWeatherRespose();
            weatherTypeRespose = new GetWeatherTypeRespose();
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

        #region 状态
        /// <summary>
        /// 使用在导航过程中传递的内容填充页。在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        /// 事件的来源；通常为 <see cref="NavigationHelper"/>。
        /// </param>
        /// <param name="e">事件数据，其中既提供在最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的导航参数，又提供
        /// 此页在以前会话期间保留的状态的
        /// 的字典。首次访问页面时，该状态将为 null。</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //通知
            //string toastText = "明天冷空气南下，请注意保暖";
            //ToastHelper.CreateToast(toastText);

        }



        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合序列化
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper"/>。</param>
        ///<param name="e">提供要使用可序列化状态填充的空字典
        ///的事件数据。</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: 在此处保存页面的唯一状态。
        }
        #endregion



        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;

            long thisTick = DateTime.Now.Ticks;
            if (LastExitAttemptTick - thisTick < 2)
            {
                //退出代码
            }
            else
            {
                LastExitAttemptTick = DateTime.Now.Ticks;
            }
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            cityId = e.Parameter.ToString();

            GetWeather(cityId, 0);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region 获取天气数据

        /// <summary>
        /// 获取天气数据
        /// </summary>
        /// <param name="cityId"></param>
        private async void GetWeather(string cityId, int flatRefresh)
        {
            Model.UserCity userCity = null;
            progressBar.Visibility = Visibility.Visible;
            userCityRespose = await userService.GetUserCityAsync();
            userCity = string.IsNullOrEmpty(cityId) ? userCityRespose.UserCities.FirstOrDefault(x => x.IsDefault == 1) : userCityRespose.UserCities.FirstOrDefault(x => x.CityId == int.Parse(cityId));
            IGetWeatherRequest weatherRequest = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.City, userCity.CityName);
            weatherTypeRespose = await weatherService.GetWeatherTypeAsync();
            if (!NetHelper.IsNetworkAvailable())
            {
                if (flatRefresh == 1)
                {
                    weatherRespose = await weatherService.GetWeatherAsync(weatherRequest);
                }
                else
                {
                    string filePath = "Temp\\" + cityId + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    if (!await FileHelper.IsExistFile(filePath))
                    {
                        //不存在当天的天气数据，就从网络获取数据
                        weatherRespose = await weatherService.GetWeatherAsync(weatherRequest);
                    }
                    else
                    {
                        weatherRespose = await weatherService.GetWeatherByClientAsync(userCity.CityId.ToString());
                    }
                }

            }
            else
            {
                weatherRespose = await weatherService.GetWeatherByClientAsync(userCity.CityId.ToString());
            }
            if (weatherRespose!=null)
            {
                weatherService.SaveWeather(weatherRespose, userCity.CityId.ToString());
                ViewModel.HomePageModel homePageModel = new ViewModel.HomePageModel();
                homePageModel.WeatherType = weatherTypeRespose.WeatherTypes.Find(x => x.Wid == weatherRespose.result.today.weather_id.fa);
                weatherRespose.result.sk.temp = weatherRespose.result.sk.temp + "°";
                weatherRespose.result.sk.time = weatherRespose.result.sk.time + "发布";
                homePageModel.Sk = weatherRespose.result.sk;
                homePageModel.Today = weatherRespose.result.today;

                //var futures1 = weatherRespose.result.future.ForEach(x => x.weather_id.fa = weatherTypeRespose.WeatherTypes.Find(w => w.Wid == x.weather_id.fa).TomorrowPic);
                //var futures2 = weatherRespose.result.future.ForEach(x => x.weather_id.fb = weatherTypeRespose.WeatherTypes.Find(w => w.Wid == x.weather_id.fa).BackgroundPic);
                homePageModel.Futures = weatherRespose.result.future.AsParallel().ForEach(x => x.weather_id.fa = weatherTypeRespose.WeatherTypes.Find(w => w.Wid == x.weather_id.fa).TomorrowPic).ToList();
                pivot.DataContext = homePageModel;
            }
            progressBar.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region 刷新+常用城市

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void abbRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetWeather(cityId, 1);
        }

        /// <summary>
        /// 常用城市
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void abbCity_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MyCityPage));
        }
        #endregion

        #region CommandBar

        private void SecondaryTileCommandBar_Click(object sender, RoutedEventArgs e)
        {
            string tileId = "ZxjWeather";
            if (!SecondaryTileHelper.IsExists(tileId))
            {
                string displayName = "天气在线";
                string tileActivationArguments = "ZxjWeather" + DateTime.Now;
                SecondaryTileHelper.CreateSecondaryTileAsync(tileId, displayName, tileActivationArguments);
            }
        }


        private void SettingCommandBar_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }
        #endregion
    }
}
