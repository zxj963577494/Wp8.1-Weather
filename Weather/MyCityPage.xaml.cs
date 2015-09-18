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
using Weather.Utils;
using Weather.App.ViewModel;
using System.Text;

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
        private UserService userService = null;
        private WeatherService weatherService;
        private GetUserRespose userRespose;
        private GetUserCityRespose userCityRespose = null;
        private GetWeatherTypeRespose weatherTypeRespose;
        private GetWeatherRespose weatherRespose = null;
        private ViewModel.MyCityPage myCityPage = null;
        IList<ViewModel.MyCityPageModel> myCityPageModelList = null;

        public MyCityPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            userService = UserService.GetInstance();
            weatherService = WeatherService.GetInstance();
            userRespose = new GetUserRespose();
            userCityRespose = new GetUserCityRespose();
            weatherTypeRespose = new GetWeatherTypeRespose();
            weatherRespose = new GetWeatherRespose();
            myCityPage = new ViewModel.MyCityPage();
            myCityPageModelList = new List<ViewModel.MyCityPageModel>();

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

            try
            {
                userCityRespose = await userService.GetUserCityAsync();

                userRespose = await userService.GetUserAsync();

                weatherTypeRespose = await weatherService.GetWeatherTypeAsync();

                if (userCityRespose != null && userRespose != null && weatherTypeRespose != null)
                {
                    await GetWeather(0);
                }
            }
            catch (Exception)
            {

                throw;
            }


        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);

        }

        #endregion

        #region 获取天气数据
        private async Task GetWeather(int isRefresh)
        {
            popupProgressBar.IsOpen = true;
            // progressBar.Visibility = Visibility.Visible;
            myCityPage.MyCityPageModels = null;
            myCityPageModelList = null;
            myCityPageModelList = new List<ViewModel.MyCityPageModel>();
            //遍历常用城市
            foreach (var item in userCityRespose.UserCities)
            {
                MyCityPageModel model = null;
                // 如果有网络
                if (NetHelper.IsNetworkAvailable())
                {
                    // 不使用wifi更新
                    if (userRespose.UserConfig.IsWifiUpdate == 0)
                    {
                        //强制刷新
                        if (isRefresh == 1)
                        {
                            // 更新所有常用城市
                            if (userRespose.UserConfig.IsUpdateForCity == 0)
                            {
                                model = await GetWeatherByNet(item);
                                myCityPageModelList.Add(model);
                            }
                            else //只更新默认城市
                            {
                                //是默认城市
                                if (item.IsDefault == 1)
                                {
                                    model = await GetWeatherByNet(item);
                                    myCityPageModelList.Add(model);
                                }
                                else
                                {
                                    //天气数据置为空
                                    model = GetWeatherByNo(item);
                                    myCityPageModelList.Add(model);
                                }
                            }
                        }
                        else
                        {
                            // 更新所有常用城市
                            if (userRespose.UserConfig.IsUpdateForCity == 0)
                            {
                                model = await GetWeatherByNetOrClient(item);
                                myCityPageModelList.Add(model);
                            }
                            else //只更新默认城市
                            {
                                //是默认城市
                                if (item.IsDefault == 1)
                                {
                                    model = await GetWeatherByNetOrClient(item);
                                    myCityPageModelList.Add(model);
                                }
                                else
                                {
                                    //天气数据置为空
                                    model = GetWeatherByNo(item);
                                    myCityPageModelList.Add(model);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (NetHelper.IsWifiConnection())
                        {
                            //强制刷新
                            if (isRefresh == 1)
                            {
                                // 更新所有常用城市
                                if (userRespose.UserConfig.IsUpdateForCity == 0)
                                {
                                    model = await GetWeatherByNet(item);
                                    myCityPageModelList.Add(model);
                                }
                                else //只更新默认城市
                                {
                                    //是默认城市
                                    if (item.IsDefault == 1)
                                    {
                                        model = await GetWeatherByNet(item);
                                        myCityPageModelList.Add(model);
                                    }
                                    else
                                    {
                                        //天气数据置为空
                                        model = GetWeatherByNo(item);
                                        myCityPageModelList.Add(model);
                                    }
                                }
                            }
                            else
                            {
                                // 更新所有常用城市
                                if (userRespose.UserConfig.IsUpdateForCity == 0)
                                {
                                    model = await GetWeatherByNetOrClient(item);
                                    myCityPageModelList.Add(model);
                                }
                                else //只更新默认城市
                                {
                                    //是默认城市
                                    if (item.IsDefault == 1)
                                    {
                                        model = await GetWeatherByNetOrClient(item);
                                        myCityPageModelList.Add(model);
                                    }
                                    else
                                    {
                                        //天气数据置为空
                                        model = GetWeatherByNo(item);
                                        myCityPageModelList.Add(model);
                                    }
                                }
                            }
                        }
                        else
                        {
                            model = GetWeatherByNo(item);
                            myCityPageModelList.Add(model);
                            NotifyUser("Wifi未启动");
                        }
                    }
                }
                else
                {
                    model = GetWeatherByNo(item);
                    myCityPageModelList.Add(model);
                    NotifyUser("请开启网络，以更新最新天气数据");
                }
                myCityPage.MyCityPageModels = myCityPageModelList.ToList();
            }
            LayoutRoot.DataContext = null;
            LayoutRoot.DataContext = myCityPage;
            popupProgressBar.IsOpen = false;
            //progressBar.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// 通过网络获取
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<MyCityPageModel> GetWeatherByNet(Model.UserCity item)
        {
            //不存在当天的天气数据，就从网络获取数据
            IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.CityId, item.CityId);
            weatherRespose = await weatherService.GetWeatherAsync(request);
            await DeleteFile(item.CityId);
            await weatherService.SaveWeather<GetWeatherRespose>(weatherRespose, item.CityId.ToString());
            MyCityPageModel model = new MyCityPageModel()
            {
                CityId = item.CityId,
                CityName = item.CityName,
                Temp = weatherRespose == null ? null : weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault().tmp.min + "°~" + weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault().tmp.max + "°",
                TodayPic = weatherRespose == null ? null : weatherTypeRespose.WeatherTypes.Find(x => x.Code == weatherRespose.result.FirstOrDefault().now.cond.code).TodayPic
            };
            return model;
        }

        /// <summary>
        /// 通过本地获取
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<MyCityPageModel> GetWeatherByClient(Model.UserCity item)
        {
            weatherRespose = await weatherService.GetWeatherByClientAsync(item.CityId.ToString()).ConfigureAwait(false);
            MyCityPageModel model = new MyCityPageModel();
            model.CityId = item.CityId;
            model.CityName = item.CityName;
            model.Temp = weatherRespose == null ? null : weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault(x => x.date == DateTime.Now.ToString("yyyy-MM-dd")).tmp.min + "°~" + weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault(x => x.date == DateTime.Now.ToString("yyyy-MM-dd")).tmp.max + "°";
            model.TodayPic = weatherRespose == null ? null : weatherTypeRespose.WeatherTypes.Find(x => x.Code == weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault(y => y.date == DateTime.Now.ToString("yyyy-MM-dd")).cond.code_d).TodayPic;
            return model;
        }


        /// <summary>
        /// 通过本地或网络方式获取
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task<MyCityPageModel> GetWeatherByNetOrClient(Model.UserCity item)
        {
            MyCityPageModel model = null;
            string filePath = StringHelper.GetTodayFilePath(item.CityId);
            bool isExistFile = await FileHelper.IsExistFileAsync(filePath);
            //存在今日天气数据
            if (isExistFile)
            {
                model = await GetWeatherByClient(item).ConfigureAwait(false);
            }
            else
            {
                model = await GetWeatherByNet(item).ConfigureAwait(false);
            }
            return model;
        }

        /// <summary>
        /// 只获取城市ID和名称
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public MyCityPageModel GetWeatherByNo(Model.UserCity item)
        {
            MyCityPageModel model = new MyCityPageModel()
            {
                CityId = item.CityId,
                CityName = item.CityName,
                Temp = null,
                TodayPic = null,
            };
            return model;
        }

        #endregion

        #region Holding

        private void gvCity_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var flyout = (MenuFlyout)Resources["MenuFlyoutSource"];
            switch (e.HoldingState)
            {
                case Windows.UI.Input.HoldingState.Started:
                    if (e.OriginalSource is TextBlock)
                    {
                        var tb = e.OriginalSource as TextBlock;
                        flyout.ShowAt(tb);
                    }
                    else if (e.OriginalSource is Image)
                    {
                        var tb = e.OriginalSource as Image;
                        flyout.ShowAt(tb);
                    }
                    else
                    {
                        foreach (var item in ((e.OriginalSource as Border).Child as Grid).Children)
                        {
                            if (item is ContentPresenter)
                            {
                                var cp = (item as ContentPresenter);
                                flyout.ShowAt(cp);
                                break;
                            }
                        }
                    }
                    break;
                case Windows.UI.Input.HoldingState.Completed:
                case Windows.UI.Input.HoldingState.Canceled:
                    break;
            }
        }


        private async void DefaultCity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuFlyoutItem selectedItem = sender as MenuFlyoutItem;
                if (selectedItem != null)
                {
                    string cityId = selectedItem.CommandParameter.ToString();
                    var DefaultCityed = userCityRespose.UserCities.FirstOrDefault(x => x.IsDefault == 1);
                    if (DefaultCityed.CityId != cityId)
                    {
                        userCityRespose.UserCities.FirstOrDefault(x => x.CityId == DefaultCityed.CityId).IsDefault = 0;
                        userCityRespose.UserCities.FirstOrDefault(x => x.CityId == cityId).IsDefault = 1;

                        GetUserCityRespose list = SortUserCity(userCityRespose);

                        myCityPage.MyCityPageModels = SortCityPageModelList(userCityRespose);

                        LayoutRoot.DataContext = null;

                        LayoutRoot.DataContext = myCityPage;

                        await userService.SaveUserCity(list);

                        NotifyUser("设置成功");

                        await GetWeather(1);

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

        private async void RemoveCity_Click(object sender, RoutedEventArgs e)
        {

            MenuFlyoutItem selectedItem = sender as MenuFlyoutItem;
            if (selectedItem != null)
            {
                if (userCityRespose.UserCities.Count > 1)
                {
                    string cityId = selectedItem.CommandParameter.ToString();

                    ViewModel.MyCityPageModel city = myCityPageModelList.FirstOrDefault(x => x.CityId == cityId);

                    if (userCityRespose.UserCities.FirstOrDefault(x => x.CityId == city.CityId).IsDefault == 1)
                    {
                        myCityPageModelList.Remove(city);
                        userCityRespose.UserCities.RemoveAll(x => x.CityId == cityId);
                        userCityRespose.UserCities.FirstOrDefault().IsDefault = 1;
                        await GetWeather(1);
                    }
                    else
                    {
                        userCityRespose.UserCities.RemoveAll(x => x.CityId == cityId);
                        myCityPageModelList.Remove(city);
                    }
                    GetUserCityRespose list = SortUserCity(userCityRespose);
                    myCityPage.MyCityPageModels = myCityPageModelList.ToList();
                    LayoutRoot.DataContext = null;
                    LayoutRoot.DataContext = myCityPage;
                    await userService.SaveUserCity(list);
                    await SecondaryTileHelper.DeleteSecondaryTileAsync(cityId + "_Weather");
                }
                else
                {
                    NotifyUser("必须保留一个城市");
                    return;
                }
            }
        }

        private async void DesTopTile_Click(object sender, RoutedEventArgs e)
        {
            //如果有网络
            if (NetHelper.IsNetworkAvailable())
            {
                MenuFlyoutItem selectedItem = sender as MenuFlyoutItem;
                if (selectedItem != null)
                {
                    string cityId = selectedItem.CommandParameter.ToString();

                    string tileId = selectedItem.CommandParameter.ToString() + "_Weather";

                    Model.UserCity userCity = (from u in userCityRespose.UserCities
                                               where u.CityId == cityId
                                               select u).FirstOrDefault();

                    string displayName = userCity.CityName;

                    IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.CityId, userCity.CityId);
                    GetWeatherRespose respose = await weatherService.GetWeatherAsync(request);
                    if (!Utils.SecondaryTileHelper.IsExists(tileId))
                    {
                        await Utils.SecondaryTileHelper.CreateSecondaryTileAsync(tileId, displayName, cityId.ToString());
                        UpdateSecondaryTile(tileId, respose.result.FirstOrDefault().daily_forecast.FirstOrDefault(), displayName);
                    }
                    else
                    {
                        NotifyUser("该城市磁贴已固定在桌面");
                        UpdateSecondaryTile(tileId, respose.result.FirstOrDefault().daily_forecast.FirstOrDefault(), displayName);
                    }
                }
            }
            else
            {
                NotifyUser("该功能需要网络连接");
            }
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

        #region 常用城市排序

        /// <summary>
        /// 常用城市排序
        /// </summary>
        /// <param name="weatherRespose"></param>
        /// <returns></returns>
        private GetUserCityRespose SortUserCity(GetUserCityRespose respose)
        {
            var data = from c in respose.UserCities
                       orderby c.IsDefault descending, c.AddTime descending
                       select c;
            respose.UserCities = data.ToList();
            return respose;
        }

        /// <summary>
        /// 常用城市磁贴排序
        /// </summary>
        /// <param name="weatherRespose"></param>
        /// <returns></returns>
        private List<MyCityPageModel> SortCityPageModelList(GetUserCityRespose respose)
        {
            respose = SortUserCity(respose);
            List<MyCityPageModel> newList = new List<MyCityPageModel>();
            foreach (var item in respose.UserCities)
            {
                MyCityPageModel model = myCityPageModelList.FirstOrDefault(x => x.CityId == item.CityId);
                newList.Add(model);
            }
            return newList;
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

        #region Command

        private void abbAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddCityPage));
        }

        private async void abbRefresh_Click(object sender, RoutedEventArgs e)
        {
            await GetWeather(1);
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

        #region ItemClick

        private void gvCity_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.MyCityPageModel city = (ViewModel.MyCityPageModel)e.ClickedItem;
            Frame.Navigate(typeof(PivotPage), city.CityId);
        }
        #endregion

        #region 磁贴更新

        private void UpdateSecondaryTile(string tileId, Model.Weather.Daily_forecastItem daily_forecast, string cityName)
        {
            if (weatherRespose.result != null)
            {
                if (SecondaryTileHelper.IsExists(tileId))
                {
                    string tileXmlString = "<tile>"
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
                + ((DateTime.Compare(DateTime.Now.ToLocalTime(), DateTime.Parse(daily_forecast.astro.sr)) > 1 & DateTime.Compare(DateTime.Now.ToLocalTime(), DateTime.Parse(daily_forecast.astro.ss)) < 0) ? weatherTypeRespose.WeatherTypes.Find(x => x.Code == daily_forecast.cond.code_d).TileSquarePic : weatherTypeRespose.WeatherTypes.Find(x => x.Code == daily_forecast.cond.code_n).TileSquarePic)
                + "'/>"
                + "<text id='1'>" + cityName + "</text>"
                + "<text id='2'>" + daily_forecast.tmp.min + "°~" + daily_forecast.tmp.max + "</text>"
                + "<text id='3'>" + (daily_forecast.cond.code_d == daily_forecast.cond.code_n ? daily_forecast.cond.txt_d : daily_forecast.cond.txt_d + "转" + daily_forecast.cond.txt_n) + "</text>"
                + "<text id='4'>" + daily_forecast.wind.dir + " " + daily_forecast.wind.sc + "</text>"
                + "</binding>"
                + "</visual>"
                + "</tile>";
                    SecondaryTileHelper.UpdateSecondaryTileNotificationsByXml(tileId, tileXmlString);
                }
            }
        }

        #endregion
    }
}
