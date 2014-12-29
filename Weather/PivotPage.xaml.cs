using Weather.App.Common;
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
using System.Threading.Tasks;

// “透视应用程序”模板在 http://go.microsoft.com/fwlink/?LinkID=391641 上有介绍

namespace Weather.App
{
    public sealed partial class PivotPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        private UserService userService;
        private WeatherService weatherService;
        private SettingService settingService;
        private GetUserRespose userRespose;
        private GetUserCityRespose userCityRespose;
        private GetWeatherRespose weatherRespose;
        private GetWeatherTypeRespose weatherTypeRespose;
        private GetSettingAutoUpdateTimeRepose settingAutoUpdateTimeRepose;
        private string cityId = null;


        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            userService = UserService.GetInstance();
            weatherService = WeatherService.GetInstance();
            settingService = SettingService.GetInstance();
            userRespose = new GetUserRespose();
            userCityRespose = new GetUserCityRespose();
            weatherRespose = new GetWeatherRespose();
            weatherTypeRespose = new GetWeatherTypeRespose();
            settingAutoUpdateTimeRepose = new GetSettingAutoUpdateTimeRepose();
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

            cityId = e.Parameter.ToString();

            userCityRespose = await userService.GetUserCityAsync();
            userRespose = await userService.GetUserAsync();
            weatherTypeRespose = await weatherService.GetWeatherTypeAsync();
            if (userCityRespose == null)
            {
                Frame.Navigate(typeof(AddCityPage), 1);
                return;
            }
            else
            {
                GetWeather(cityId, 0);
            }
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
        private async void GetWeather(string cityId, int isRefresh)
        {
            progressBar.Visibility = Visibility.Visible;



            Model.UserCity userCity = string.IsNullOrEmpty(cityId) ? userCityRespose.UserCities.FirstOrDefault(x => x.IsDefault == 1) : userCityRespose.UserCities.FirstOrDefault(x => x.CityId == int.Parse(cityId));

            IGetWeatherRequest weatherRequest = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.City, userCity.CityName);


           

            //有网络
            if (NetHelper.IsNetworkAvailable())
            {
                if (userRespose.UserConfig.IsWifiUpdate == 0)
                {
                    weatherRespose = await GetWeatherAsync(isRefresh, userCity, weatherRequest);
                }
                else
                {
                    if (NetHelper.IsWifiConnection())
                    {
                        weatherRespose = await GetWeatherAsync(isRefresh, userCity, weatherRequest);
                    }
                    else
                    {
                        NotifyUser("Wifi未启动");
                        weatherRespose = await GetWeatherAsync(isRefresh, userCity, weatherRequest);
                    }
                }
            }
            else
            {
                weatherRespose = await weatherService.GetWeatherByClientAsync(userCity.CityId.ToString());
                NotifyUser("请开启网络，以更新最新天气数据");
            }

            if (weatherRespose.result != null)
            {
                await DeleteFile(userCity.CityId);
                await weatherService.SaveWeather(weatherRespose, userCity.CityId.ToString());
                ViewModel.HomePageModel homePageModel = new ViewModel.HomePageModel();
                homePageModel.WeatherType = weatherTypeRespose.WeatherTypes.Find(x => x.Wid == weatherRespose.result.today.weather_id.fa);
                weatherRespose.result.sk.temp = weatherRespose.result.sk.temp + "°";
                weatherRespose.result.sk.time = weatherRespose.result.sk.time + "发布";
                homePageModel.Sk = weatherRespose.result.sk;
                homePageModel.Today = weatherRespose.result.today;
                homePageModel.Futures = weatherRespose.result.future.AsParallel().ForEach(x => x.weather_id.fa = weatherTypeRespose.WeatherTypes.Find(w => w.Wid == x.weather_id.fa).TomorrowPic).ToList();
                LayoutRoot.DataContext = homePageModel;


                UpdateTileFacade(weatherRespose);
            }

            progressBar.Visibility = Visibility.Collapsed;
        }




        private async Task<GetWeatherRespose> GetWeatherAsync(int isRefresh, Model.UserCity userCity, IGetWeatherRequest weatherRequest)
        {
            GetWeatherRespose weatherRespose = new GetWeatherRespose();

            if (isRefresh == 1)
            {
                weatherRespose = await weatherService.GetWeatherAsync(weatherRequest);
            }
            else
            {
                string filePath = StringHelper.GetTodayFilePath(userCity.CityId);

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
            return weatherRespose;
        }


        #endregion

        #region 删除过期文件

        /// <summary>
        /// 删除过期文件
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        private async Task DeleteFile(int cityId)
        {
            string fileName = cityId + "_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".txt";
            string filePath = "Temp\\" + fileName;
            bool x = await FileHelper.IsExistFile(filePath);
            if (x)
            {
                await FileHelper.DeleteFile(filePath);

            }
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

        private async void SecondaryTileCommandBar_Click(object sender, RoutedEventArgs e)
        {
            Model.UserCity userCity = userCityRespose.UserCities.FirstOrDefault(x => x.IsDefault == 1);

            string tileId = userCity.CityId + "_Weather";
            if (!SecondaryTileHelper.IsExists(tileId))
            {
                string displayName = userCity.CityName;
                string tileActivationArguments = userCity.CityId.ToString();
              await SecondaryTileHelper.CreateSecondaryTileAsync(tileId, displayName, tileActivationArguments);
            }
            else
            {
                NotifyUser("该城市磁贴已固定在桌面");
            }
        }


        private void SettingCommandBar_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }

        private void AboutCommandBar_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }
        #endregion

        #region 消息通知

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
        #endregion

        /// <summary>
        /// 获取默认城市
        /// </summary>
        /// <returns></returns>
        private async Task<Model.UserCity> GetDefaultCity()
        {
            UserService userService = UserService.GetInstance();
            GetUserCityRespose userRespose = await userService.GetUserCityAsync();
            if (userRespose.UserCities != null)
            {
                return userRespose.UserCities.Find(x => x.IsDefault == 1);
            }
            else
            {
                return null;
            }
        }

        #region 更新磁贴

        private async void UpdateTileFacade(GetWeatherRespose weatherRespose)
        {

            Model.UserCity defaultCity = await GetDefaultCity();

            if (weatherRespose.result.today.date_y == DateTime.Now.ToString("yyyy年MM月dd日"))
            {
                UpdateTile(weatherRespose);
            }
            else
            {
                Model.Future future = weatherRespose.result.future.Find(x => x.date == StringHelper.GetTodayDateString());
                UpdateTileByClientForTomorrow(future, defaultCity.CityName);
            }
        }

        /// <summary>
        /// 更新磁贴
        /// </summary>
        /// <param name="respose"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTile(GetWeatherRespose respose)
        {

            //          string tileXmlString = "<tile>"
            //   + "<visual version='2'>"
            //   + "<binding template='TileWide310x150PeekImage03' fallback='TileWidePeekImage03'>"
            //   + "<image id='1' src='ms-appx:///" + (userRespose.UserConfig.IsTileSquarePic == 1 ? weatherTypeRespose.WeatherTypes.Find(x => x.Wid == weatherRespose.result.today.weather_id.fa).TileWidePic : "Assets/Logo.png") + "'/>"
            //   + "<text id='1'>" + weatherRespose.result.today.city + "\r\n" + weatherRespose.result.sk.temp + "(" + weatherRespose.result.today.temperature + ")\r\n" + weatherRespose.result.today.weather + "</text>"
            //   + "</binding>"
            //+ "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
            //+ "<image id='1' src='ms-appx:///" + (userRespose.UserConfig.IsTileSquarePic == 1 ? weatherTypeRespose.WeatherTypes.Find(x => x.Wid == weatherRespose.result.today.weather_id.fa).TileSquarePic : "Assets/Logo.png") + "'/>"
            //+ "<text id='1'>" + weatherRespose.result.sk.temp + "</text>"
            //+ "<text id='2'>" + weatherRespose.result.today.weather + "</text>"
            //+ "<text id='3'>" + weatherRespose.result.today.temperature + "</text>"
            //+ "<text id='4'>" + weatherRespose.result.sk.wind_direction + " " + weatherRespose.result.sk.wind_strength + "</text>"
            //+ "</binding>"
            //+ "</visual>"
            //+ "</tile>";

            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
               + "<text id='1'>" + respose.result.sk.temp + "</text>"
               + "<text id='2'>" + respose.result.today.city + "</text>"
               + "<text id='3'>" + respose.result.today.weather + "</text>"
               + "<text id='4'>" + respose.result.today.temperature + "</text>"
               + "<text id='5'>" + respose.result.sk.wind_direction + " " + respose.result.sk.wind_strength + "</text>"
               + "<text id='6'>" + respose.result.today.week + "</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + (userRespose.UserConfig.IsTileSquarePic == 1 ? weatherTypeRespose.WeatherTypes.Find(x => x.Wid == respose.result.today.weather_id.fa).TileSquarePic : "Assets/Logo.png") + "'/>"
               + "<text id='1'>" + respose.result.today.city + "</text>"
               + "<text id='2'>" + respose.result.today.weather + "</text>"
               + "<text id='3'>" + respose.result.sk.temp + "</text>"
               + "<text id='4'>" + respose.result.sk.wind_direction + " " + respose.result.sk.wind_strength + "</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            TileHelper.UpdateTileNotificationsByXml(tileXmlString);
        }

        /// <summary>
        /// 通过本地更新磁贴，未来天气
        /// </summary>
        /// <param name="future"></param>
        /// <param name="cityName"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTileByClientForTomorrow(Model.Future future, string cityName)
        {
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
               + "<text id='1'>暂无</text>"
               + "<text id='2'>" + cityName + "</text>"
               + "<text id='3'>" + future.weather + "</text>"
               + "<text id='4'>" + future.temperature + "</text>"
               + "<text id='5'>" + future.wind + "</text>"
               + "<text id='6'>" + future.week + "</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + (userRespose.UserConfig.IsTileSquarePic == 1 ? weatherTypeRespose.WeatherTypes.Find(x => x.Wid == future.weather_id.fa).TileSquarePic : "Assets/Logo.png") + "'/>"
               + "<text id='1'>" + cityName + "</text>"
               + "<text id='2'>" + future.weather + "</text>"
               + "<text id='3'>" + future.temperature + "</text>"
               + "<text id='4'>" + future.wind + "</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            TileHelper.UpdateTileNotificationsByXml(tileXmlString);
        }
        #endregion

        private void InstructionCommandBar_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Instruction));
            return;
        }

        private void EvaluateCommandBar_Click(object sender, RoutedEventArgs e)
        {
            SettingPageHelper.LaunchUriForMarketplaceDetail();
        }



    }
}
