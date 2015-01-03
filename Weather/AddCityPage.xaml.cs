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
using System.Collections.ObjectModel;
using Weather.Service.Message;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Weather.Service.Implementations;
using Weather.Utils;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Weather.App
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AddCityPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ViewModel.SelectCityPage page = null;
        private CityService cityService = null;
        private UserService userService = null;
        private GetCityRespose resposeCities = null;
        private GetHotCityRespose resposeHotCities = null;
        private GetUserCityRespose resposeUserCity = null;

        bool isNotFirst = true;


        public AddCityPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件

            cityService = CityService.GetInstance();
            userService = UserService.GetInstance();
            resposeCities = new GetCityRespose();
            resposeHotCities = new GetHotCityRespose();
            resposeUserCity = new GetUserCityRespose();
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
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            resposeUserCity = await userService.GetUserCityAsync();

            if (resposeUserCity==null)
            {
                isNotFirst = false;
            }

            resposeCities = await cityService.GetCityAsync();

            resposeHotCities = await cityService.GetHotCityAsync(); ;

            page = new ViewModel.SelectCityPage();
            page.Cities = resposeCities.Cities;
            page.HotCities = resposeHotCities.Cities;
            LayoutRoot.DataContext = page;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (isNotFirst)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame != null && rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                    e.Handled = true;
                }
            }
            else
            {
                Application.Current.Exit();
            }
        }
        #endregion


        private void asbCity_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string userInput = sender.Text.Trim();
            if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
            {
                return;
            }
            if (userInput.Length == 0)
            {
                return;
            }
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = from d in page.Cities
                                     where d.District.Contains(userInput)
                                     select d.District;
            }
        }



        private async void asbCity_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

            string cityname = args.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(cityname))
            {
                bool isSuccess = await UpdateUserCity(cityname);
                if (isSuccess)
                {
                    Frame.Navigate(typeof(MyCityPage));
                    return;
                }
            }

        }

        private async void gvCity_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {

                Model.WeatherCity city = (Model.WeatherCity)e.ClickedItem;
                if (!string.IsNullOrEmpty(city.District))
                {
                    bool isSuccess = await UpdateUserCity(city.District);
                    if (isSuccess)
                    {
                        Frame.Navigate(typeof(MyCityPage));
                        return;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<bool> UpdateUserCity(string cityName)
        {
            bool isAdd = false;

            Model.UserCity userCity = new Model.UserCity()
            {
                CityId = (from c in page.Cities
                          where c.District == cityName
                          select c.Id).FirstOrDefault(),
                AddTime = DateTime.Now,
                CityName = cityName.Trim(),
                IsDefault = isNotFirst == false ? 1 : 0
            };
            if (!isNotFirst)
            {
                GetUserCityRespose respose = new GetUserCityRespose();
                List<Model.UserCity> userCityList = new List<Model.UserCity>();
                userCityList.Add(userCity);
                respose.UserCities = userCityList;
                await userService.SaveUserCity(SortUserCity(respose));
                isAdd = true;
            }
            else
            {
                if (resposeUserCity.UserCities.Count < 5)
                {
                    var count = resposeUserCity.UserCities.Count(x => x.CityName.Contains(cityName.Trim()));

                    if (count == 0)
                    {
                        resposeUserCity.UserCities.Add(userCity);

                        await userService.SaveUserCity(SortUserCity(resposeUserCity));
                        isAdd = true;
                    }
                    else
                    {
                        NotifyUser("该城市已加入常用城市列表");
                    }
                }
                else
                {
                    NotifyUser("因资源有限，每人最多拥有5座常用城市");
                }
            }

            return isAdd;
        }
        private GetUserCityRespose SortUserCity(GetUserCityRespose respose)
        {
            var data = from c in respose.UserCities
                       orderby c.IsDefault descending, c.AddTime descending
                       select c;
            respose.UserCities = data.ToList();
            return respose;
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
    }
}
