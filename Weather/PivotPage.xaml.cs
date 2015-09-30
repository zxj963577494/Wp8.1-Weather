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
using Windows.Phone.UI.Input;
using Weather.App.ViewModel;

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

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;

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
            cityId = e.Parameter == null ? "" : e.Parameter.ToString();
            this.navigationHelper.OnNavigatedTo(e);

            userCityRespose = await userService.GetUserCityAsync();

            if (userCityRespose == null)
            {
                Frame.Navigate(typeof(AddCityPage), 1);
                return;
            }
            else
            {
                if (cityId == "")
                {
                    cityId = userCityRespose.UserCities.FirstOrDefault().CityId;
                }
                userRespose = await userService.GetUserAsync();
                weatherTypeRespose = await weatherService.GetWeatherTypeAsync();
                await GetWeather(cityId, 0);
            }

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            ApplicationHelper.Exit();
        }

        #endregion

        #region 获取天气数据

        /// <summary>
        /// 获取天气数据
        /// </summary>
        /// <param name="cityId"></param>
        private async Task GetWeather(string cityId, int isRefresh)
        {
            popupProgressBar.IsOpen = true;

            Model.UserCity userCity = string.IsNullOrEmpty(cityId) ? userCityRespose.UserCities.FirstOrDefault(x => x.IsDefault == 1) : userCityRespose.UserCities.FirstOrDefault(x => x.CityId == cityId);

            IGetWeatherRequest weatherRequest = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.CityId, userCity.CityId);

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
                        weatherRespose.result = null;
                        NotifyUser("Wifi未启动");
                    }
                }
            }
            else
            {
                weatherRespose = await weatherService.GetWeatherByClientAsync(userCity.CityId.ToString());
                NotifyUser("请开启网络，以更新最新天气数据");
            }

            if (weatherRespose != null)
            {
                var respose = weatherRespose.result.FirstOrDefault();

                ViewModel.HomePageModel homePageModel = new ViewModel.HomePageModel();

                var aqi = respose.aqi;
                var now = respose.now;
                var basic = respose.basic;
                var daily_forecast = respose.daily_forecast;
                var hourly_forecast = respose.hourly_forecast;
                if (aqi != null)
                {
                    homePageModel.Aqi = "空气质量: " + aqi.city.qlty;
                }
                homePageModel.City = basic.city;
                homePageModel.Daytmp = daily_forecast.FirstOrDefault().tmp.min + "° / " + daily_forecast.FirstOrDefault().tmp.max + "°";
                homePageModel.Hum = now.hum + " %";
                homePageModel.Pres = now.pres + " hPa";
                homePageModel.Tmp = now.tmp + "°";
                homePageModel.Txt = now.cond.txt;
                homePageModel.Update = basic.update.loc + " 发布";
                homePageModel.Vis = now.vis + " km";
                homePageModel.Wind = now.wind.dir + " " + now.wind.sc + " 级";
                homePageModel.WeatherType = weatherTypeRespose.WeatherTypes.FirstOrDefault(x => x.Code == now.cond.code);

                List<ViewModel.DailyItem> dailyList = new List<ViewModel.DailyItem>();
                foreach (var item in daily_forecast)
                {
                    DailyItem daily = new DailyItem()
                    {
                        Date = item.date,
                        Image = weatherTypeRespose.WeatherTypes.FirstOrDefault(x => x.Code == item.cond.code_d).TomorrowPic,
                        Tmp = item.tmp.min + "° / " + item.tmp.max + "°",
                        Txt = item.cond.txt_d
                    };
                    dailyList.Add(daily);
                }

                homePageModel.DailyList = dailyList;

                List<ViewModel.HourlyItem> hourlyList = new List<ViewModel.HourlyItem>();
                foreach (var item in hourly_forecast)
                {
                    HourlyItem hourly = new HourlyItem()
                    {
                        Date = DateTime.Parse(item.date).ToString("HH:mm"),
                        Hum = item.hum + " %",
                        Tmp = item.tmp + "°",
                        Wind = item.wind.dir + " " + item.wind.sc
                    };
                    hourlyList.Add(hourly);
                }
                homePageModel.HourlyList = hourlyList;

                LayoutRoot.DataContext = homePageModel;
                UpdateTileFacade();
                UpdateSecondaryTileFacade();
            }
            else
            {
                NotifyUser("天气数据获取失败");
            }

            popupProgressBar.IsOpen = false;
        }


        private async Task<GetWeatherRespose> GetWeatherAsync(int isRefresh, Model.UserCity userCity, IGetWeatherRequest weatherRequest)
        {
            GetWeatherRespose weatherRespose = new GetWeatherRespose();

            if (isRefresh == 1)
            {
                weatherRespose = await weatherService.GetWeatherAsync(weatherRequest);
                await weatherService.SaveWeather(weatherRespose, userCity.CityId.ToString());
            }
            else
            {
                string filePath = StringHelper.GetTodayFilePath(userCity.CityId);

                if (!await FileHelper.IsExistFileAsync(filePath))
                {
                    //不存在当天的天气数据，就从网络获取数据
                    weatherRespose = await weatherService.GetWeatherAsync(weatherRequest);
                    await DeleteFile(userCity.CityId);
                    await weatherService.SaveWeather(weatherRespose, userCity.CityId.ToString());
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
        private async Task DeleteFile(string cityId)
        {
            string fileName = cityId + "_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".json";
            string filePath = "Temp\\" + fileName;
            bool x = await FileHelper.IsExistFileAsync(filePath);
            if (x)
            {
                await FileHelper.DeleteFileAsync(filePath);

            }
        }
        #endregion

        #region 刷新+常用城市

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void abbRefresh_Click(object sender, RoutedEventArgs e)
        {
            await GetWeather(cityId, 1);
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
            await CreateAndUpdateSecondaryTileFacade();
        }


        private void SettingCommandBar_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }


        private void InstructionCommandBar_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Instruction));
            return;
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

        #region 获取默认城市

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
        #endregion

        #region 更新磁贴

        /// <summary>
        /// 更新应用磁贴
        /// </summary>
        private async void UpdateTileFacade()
        {
            Model.UserCity defaultCity = await GetDefaultCity();

            if (weatherRespose.result.FirstOrDefault().basic.city == defaultCity.CityName)
            {
                // 同一天
                if (weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault().date == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    UpdateTile();
                }
                else
                {
                    UpdateTileByClientForTomorrow(weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault(x => x.date == DateTime.Now.ToString("yyyy-MM-dd")), weatherRespose.result.FirstOrDefault().basic.city);

                }
            }
        }

        /// <summary>
        /// 更新辅助磁贴
        /// </summary>
        private void UpdateSecondaryTileFacade()
        {
            Model.UserCity userCity = null;
            if (string.IsNullOrEmpty(cityId))
            {
                userCity = userCityRespose.UserCities.FirstOrDefault(x => x.IsDefault == 1);
            }
            else
            {
                userCity = userCityRespose.UserCities.FirstOrDefault(x => x.CityId == cityId);
            }
            string tileId = userCity.CityId + "_Weather";
            if (SecondaryTileHelper.IsExists(tileId))
            {
                UpdateSecondaryTile(tileId);
            }
        }

        /// <summary>
        /// 创建并更新辅助磁贴
        /// </summary>
        private async Task CreateAndUpdateSecondaryTileFacade()
        {
            Model.UserCity userCity = null;
            if (string.IsNullOrEmpty(cityId))
            {
                userCity = userCityRespose.UserCities.FirstOrDefault(x => x.IsDefault == 1);
            }
            else
            {
                userCity = userCityRespose.UserCities.FirstOrDefault(x => x.CityId == cityId);
            }
            string tileId = userCity.CityId + "_Weather";
            string displayName = userCity.CityName;
            if (!Utils.SecondaryTileHelper.IsExists(tileId))
            {
                await Utils.SecondaryTileHelper.CreateSecondaryTileAsync(tileId, displayName, cityId.ToString());
                UpdateSecondaryTile(tileId);
            }
            else
            {
                NotifyUser("该城市磁贴已固定在桌面");
                UpdateSecondaryTile(tileId);
            }
        }


        /// <summary>
        /// 应用磁贴
        /// </summary>
        /// <param name="weatherRespose"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTile()
        {
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150PeekImage02' fallback='TileWidePeekImage02'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Code == weatherRespose.result.FirstOrDefault().now.cond.code).TileWidePic + "'/>"
               + "<text id='1'>" + weatherRespose.result.FirstOrDefault().basic.city + "</text>"
               + "<text id='2'>" + weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault().tmp.min + "°~" + weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault().tmp.max + "° " + weatherRespose.result.FirstOrDefault().now.cond.txt + "</text>"
               + "<text id='3'>" + weatherRespose.result.FirstOrDefault().now.wind.dir + " " + weatherRespose.result.FirstOrDefault().now.wind.sc + " 级</text>"
               + "<text id='4'>湿度: " + weatherRespose.result.FirstOrDefault().now.hum + "%</text>"
               + "<text id='5'>能见度: " + weatherRespose.result.FirstOrDefault().now.vis + "km</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Code == weatherRespose.result.FirstOrDefault().now.cond.code).TileSquarePic + "'/>"
               + "<text id='1'>" + weatherRespose.result.FirstOrDefault().basic.city + "</text>"
               + "<text id='2'>" + weatherRespose.result.FirstOrDefault().now.tmp + "°</text>"
               + "<text id='3'>" + weatherRespose.result.FirstOrDefault().now.cond.txt + "</text>"
               + "<text id='4'>" + weatherRespose.result.FirstOrDefault().now.wind.dir + " " + weatherRespose.result.FirstOrDefault().now.wind.sc + " 级</text>"
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
        private void UpdateTileByClientForTomorrow(Model.Weather.Daily_forecastItem daily_forecast, string cityName)
        {
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150Text01' fallback='TileWideText01'>"
               + "<text id='1'>" + cityName + "</text>"
               + "<text id='2'>" + daily_forecast.tmp.min + "°~" + daily_forecast.tmp.max + "° " + (daily_forecast.cond.code_d == daily_forecast.cond.code_n ? daily_forecast.cond.txt_d : daily_forecast.cond.txt_d + "转" + daily_forecast.cond.txt_n) + "</text>"
               + "<text id='3'>" + daily_forecast.wind.dir + " " + daily_forecast.wind.sc + " 级</text>"
               + "<text id='4'>湿度: " + daily_forecast.hum + "%</text>"
               + "<text id='5'>能见度: " + daily_forecast.vis + "km</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///"
               + ((DateTime.Compare(DateTime.Now.ToLocalTime(), DateTime.Parse(daily_forecast.astro.sr)) > 0 & DateTime.Compare(DateTime.Now.ToLocalTime(), DateTime.Parse(daily_forecast.astro.ss)) < 0) ? weatherTypeRespose.WeatherTypes.FirstOrDefault(x => x.Code == daily_forecast.cond.code_d).TileSquarePic : weatherTypeRespose.WeatherTypes.FirstOrDefault(x => x.Code == daily_forecast.cond.code_n).TileSquarePic)
               + "'/>"
               + "<text id='1'>" + cityName + "</text>"
               + "<text id='2'>" + daily_forecast.tmp.min + "°~" + daily_forecast.tmp.max + "</text>"
               + "<text id='3'>" + (daily_forecast.cond.code_d == daily_forecast.cond.code_n ? daily_forecast.cond.txt_d : daily_forecast.cond.txt_d + "转" + daily_forecast.cond.txt_n) + "</text>"
               + "<text id='4'>" + daily_forecast.wind.dir + " " + daily_forecast.wind.sc + " 级</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            TileHelper.UpdateTileNotificationsByXml(tileXmlString);
        }


        /// <summary>
        /// 辅助磁贴
        /// </summary>
        /// <param name="tileId"></param>
        private void UpdateSecondaryTile(string tileId)
        {
            string tileXmlString = @"<tile>"
              + "<visual version='2'>"
              + "<binding template='TileWide310x150PeekImage02' fallback='TileWidePeekImage02'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Code == weatherRespose.result.FirstOrDefault().now.cond.code).TileWidePic + "'/>"
               + "<text id='1'>" + weatherRespose.result.FirstOrDefault().basic.city + "</text>"
               + "<text id='2'>" + weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault().tmp.min + "°~" + weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault().tmp.max + "° " + weatherRespose.result.FirstOrDefault().now.cond.txt + "</text>"
               + "<text id='3'>" + weatherRespose.result.FirstOrDefault().now.wind.dir + " " + weatherRespose.result.FirstOrDefault().now.wind.sc + " 级</text>"
               + "<text id='4'>湿度: " + weatherRespose.result.FirstOrDefault().now.hum + "%</text>"
                + "<text id='5'>能见度: " + weatherRespose.result.FirstOrDefault().now.vis + "</text>"
               + "</binding>"
              + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Code == weatherRespose.result.FirstOrDefault().now.cond.code).TileSquarePic + "'/>"
               + "<text id='1'>" + weatherRespose.result.FirstOrDefault().basic.city + "</text>"
               + "<text id='2'>" + weatherRespose.result.FirstOrDefault().now.tmp + "°</text>"
               + "<text id='3'>" + weatherRespose.result.FirstOrDefault().now.cond.txt + "</text>"
               + "<text id='4'>" + weatherRespose.result.FirstOrDefault().now.wind.dir + " " + weatherRespose.result.FirstOrDefault().now.wind.sc + "</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            SecondaryTileHelper.UpdateSecondaryTileNotificationsByXml(tileId, tileXmlString);
        }

        #endregion
    }
}
