﻿using Weather.App.Common;
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
                userRespose = await userService.GetUserAsync();
                weatherTypeRespose = await weatherService.GetWeatherTypeAsync();
                GetWeather(cityId, 0);
            }


            await weatherService.GetWeatherByCityNameAsync();

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
        private async void GetWeather(string cityId, int isRefresh)
        {
            progressBar.Visibility = Visibility.Visible;


            Model.UserCity userCity = string.IsNullOrEmpty(cityId) ? userCityRespose.UserCities.FirstOrDefault(x => x.IsDefault == 1) : userCityRespose.UserCities.FirstOrDefault(x => x.CityId == int.Parse(cityId));

            GetWeatherRequest weatherRequest = new GetWeatherRequest(userCity.CityName);

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

            if (weatherRespose.result != null)
            {
                await DeleteFile(userCity.CityId);
                await weatherService.SaveWeather(weatherRespose, userCity.CityId.ToString());
                ViewModel.HomePageModel homePageModel = new ViewModel.HomePageModel();

                homePageModel.WeatherType = weatherTypeRespose.WeatherTypes.Find(x => x.Wid == weatherRespose.result.data.realtime.weather.img);

                homePageModel.Life = weatherRespose.result.data.life;

                weatherRespose.result.data.realtime.weather.temperature += "°";

                weatherRespose.result.data.realtime.time = weatherRespose.result.data.realtime.time.Substring(0, 5) + "发布";

                weatherRespose.result.data.realtime.wind.direct = weatherRespose.result.data.realtime.wind.direct + " " + weatherRespose.result.data.realtime.wind.power;

                homePageModel.Realtime = weatherRespose.result.data.realtime;

                homePageModel.PM25 = weatherRespose.result.data.pm25;



                homePageModel.WeatherList = (weatherRespose.result.data.weather.ForEach(x => x.info.day[0] = weatherTypeRespose.WeatherTypes.Find(w => w.Wid.ToString() == x.info.day[0]).TomorrowPic)).ToList();
                homePageModel.WeatherList = (weatherRespose.result.data.weather.ForEach(x => x.info.day[2] = x.info.day[2] + "°")).ToList();
                homePageModel.WeatherList = (homePageModel.WeatherList.ForEach(x => x.info.night[0] = weatherTypeRespose.WeatherTypes.Find(w => w.Wid.ToString() == x.info.night[0]).TomorrowPic)).ToList();
                homePageModel.WeatherList = (weatherRespose.result.data.weather.ForEach(x => x.info.night[2] = x.info.night[2] + "°")).ToList();
                homePageModel.WeatherList = (homePageModel.WeatherList.ForEach(x => x.date = DateTime.Parse(x.date).ToString("MM-dd"))).ToList();
                homePageModel.WeatherList = (homePageModel.WeatherList.ForEach(x => x.week = StringHelper.ConvertWeekCn(x.week))).ToList();

                homePageModel.Today = homePageModel.WeatherList.Find(x => x.date == DateTime.Now.ToString("MM-dd"));

                homePageModel.WeatherList.RemoveAt(0);

                LayoutRoot.DataContext = homePageModel;
                UpdateTileFacade();
                UpdateSecondaryTileFacade();
            }
            progressBar.Visibility = Visibility.Collapsed;
        }




        private async Task<GetWeatherRespose> GetWeatherAsync(int isRefresh, Model.UserCity userCity, GetWeatherRequest weatherRequest)
        {
            GetWeatherRespose weatherRespose = new GetWeatherRespose();

            if (isRefresh == 1)
            {
                weatherRespose = await weatherService.GetWeatherAsync(weatherRequest);
            }
            else
            {
                string filePath = StringHelper.GetTodayFilePath(userCity.CityId);

                if (!await FileHelper.IsExistFileAsync(filePath))
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

        private void EvaluateCommandBar_Click(object sender, RoutedEventArgs e)
        {
            SettingPageHelper.LaunchUriForMarketplaceDetail();
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
            if (weatherRespose.result.data.realtime.city_name == defaultCity.CityName)
            {
                if (weatherRespose.result.data.realtime.date == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    UpdateTile(weatherRespose.result.data);
                }
                else
                {
                    Model.WeatherList.Weather weather = weatherRespose.result.data.weather.Find(x => x.date == DateTime.Now.ToString("yyyy-MM-dd"));
                    UpdateTileByClientForTomorrow(weather, defaultCity.CityName);
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
                userCity = userCityRespose.UserCities.FirstOrDefault(x => x.CityId == int.Parse(cityId));
            }
            string tileId = userCity.CityId + "_Weather";
            if (SecondaryTileHelper.IsExists(tileId))
            {
                UpdateSecondaryTile(tileId, weatherRespose.result.data);
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
                userCity = userCityRespose.UserCities.FirstOrDefault(x => x.CityId == int.Parse(cityId));
            }
            string tileId = userCity.CityId + "_Weather";
            string displayName = userCity.CityName;
            if (!Utils.SecondaryTileHelper.IsExists(tileId))
            {
                await Utils.SecondaryTileHelper.CreateSecondaryTileAsync(tileId, displayName, cityId.ToString());
                UpdateSecondaryTile(tileId, weatherRespose.result.data);
            }
            else
            {
                NotifyUser("该城市磁贴已固定在桌面");
                UpdateSecondaryTile(tileId, weatherRespose.result.data);
            }
        }


        /// <summary>
        /// 应用磁贴
        /// </summary>
        /// <param name="respose"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTile(Model.Data data)
        {

            string quality = null;
            if (data.pm25 != null)
            {
                quality = data.pm25.pm25.quality;
            }
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
               + "<text id='1'>" + data.realtime.weather.temperature + "</text>"
               + "<text id='2'>" + data.realtime.city_name + "</text>"
               + "<text id='3'>" + data.realtime.weather.temperature + " " + data.realtime.weather.info + "</text>"
               + "<text id='4'>" + data.realtime.wind.direct + " " + quality + "</text>"
               + "<text id='5'>" + data.realtime.moon + "</text>"
               + "<text id='6'>" + StringHelper.ConvertWeekNum(data.realtime.week) + "</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Wid == data.realtime.weather.img).TileSquarePic + "'/>"
               + "<text id='1'>" + data.realtime.city_name + "</text>"
               + "<text id='2'>" + data.realtime.weather.temperature + "</text>"
               + "<text id='3'>" + data.realtime.weather.info + "</text>"
               + "<text id='4'>" + data.realtime.wind.direct + " " + quality + "</text>"
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
        private void UpdateTileByClientForTomorrow(Model.WeatherList.Weather weather, string cityName)
        {
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
               + "<text id='1'>" + weather.info.day[2] + "°</text>"
               + "<text id='2'>" + cityName + "</text>"
               + "<text id='3'>" + weather.info.day[1] + "</text>"
               + "<text id='4'>" + weather.info.day[3] + weather.info.day[4] + "</text>"
               + "<text id='5'>" + weather.info.night + "</text>"
               + "<text id='6'>星期" + weather.week + "</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Wid.ToString() == weather.info.day.First()).TileSquarePic + "'/>"
               + "<text id='1'>" + cityName + "</text>"
               + "<text id='2'>" + weather.info.day[2] + "°</text>"
               + "<text id='3'>" + weather.info.day[1] + "</text>"
               + "<text id='4'>" + weather.info.day[3] + weather.info.day[4] + "</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            TileHelper.UpdateTileNotificationsByXml(tileXmlString);
        }


        /// <summary>
        /// 辅助磁贴
        /// </summary>
        /// <param name="tileId"></param>
        private void UpdateSecondaryTile(string tileId, Model.Data data)
        {
            string quality = null;
            if (data.pm25 != null)
            {
                quality =data.pm25.pm25.quality;
            }
            string tileXmlString = @"<tile>"
+ "<visual version='2'>"
+ "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
+ "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Wid == data.realtime.weather.img).TileSquarePic + "'/>"
+ "<text id='1'>" + data.realtime.weather.temperature + "</text>"
+ "<text id='2'>" + data.realtime.weather.info + "</text>"
+ "<text id='3'>" + data.realtime.wind.direct + "</text>"
+ "<text id='4'>" + data.realtime.moon + " " + quality + "</text>"
+ "</binding>"
+ "</visual>"
+ "</tile>";
            SecondaryTileHelper.UpdateSecondaryTileNotificationsByXml(tileId, tileXmlString);
        }

        #endregion
    }
}
