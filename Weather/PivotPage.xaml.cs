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
        private const string FirstGroupName = "FirstGroup";
        private const string SecondGroupName = "SecondGroup";

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
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
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {




            //foreach (var task in BackgroundTaskRegistration.AllTasks)
            //{
            //    if (task.Value.Name == BackgroundTaskSample.SampleBackgroundTaskName)
            //    {
            //        AttachProgressAndCompletedHandlers(task.Value);
            //        BackgroundTaskSample.UpdateBackgroundTaskStatus(BackgroundTaskSample.SampleBackgroundTaskName, true);
            //        break;
            //    }
            //}


            //BackgroundTask.BackgroundTaskExecute execute = new BackgroundTask.BackgroundTaskExecute();
            //execute.Crete("ZXJUpdateForTile", "Weather.Tasks.UpdateTileTask", new TimeTrigger(15, false), null);


            //execute.RegisterBackgroundTask();

            //BackgroundTask.BackgroundTaskTaskModel task = new BackgroundTask.BackgroundTaskTaskModel();
            //task.BackgroundTaskName = "UpdateTileTask";
            //task.BackgroundTaskEntryPoint = "Weather.BackgroundTask.UpdateTileTask";
            //task.


            /* 通知
            string toastText = "明天冷空气南下，请注意保暖";
            Common.ToastHelper.CreateToast(toastText);
            */

            //await Common.FileHelper.IsExistFile("Temp", "20141210.txt");
            //Message.General.GetWeatherRespose respose = new Message.General.GetWeatherRespose();
            //respose = await Common.JsonSerializeHelper.JsonDeSerializeForFile<Message.General.GetWeatherRespose>("20141210.txt", "Temp");

            //Message.Forecast3h.GetForecast3hByCityNameRepose respose = new Message.Forecast3h.GetForecast3hByCityNameRepose();
            //respose = await Common.JsonSerializeHelper.JsonDeSerializeForFile<Message.Forecast3h.GetForecast3hByCityNameRepose>("20141210050000.txt", "Temp");


            //string cityname = "杭州";

            if (NetHelper.IsNetworkAvailable())
            {
                //GetWeatherRespose respose = new GetWeatherRespose();
                //IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.City, cityname);
                //string requestUrl = request.GetRequestUrl();
                //string resposeString = await Weather.Utils.HttpHelper.GetUrlRespose(requestUrl);
                //respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(resposeString);
                //await FileHelper.CreateFileForFolder("Temp", DateTime.Now.ToString("yyyyMMdd"), resposeString);

                #region 后台更新任务

                UserService userService = new UserService();
                GetUserRespose userRespose = await userService.GetUserAsync();
                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (userRespose.UserConfig.IsAutoUpdate == "1")
                {
                    SettingService settingService = new SettingService();
                    GetSettingAutoUpdateTimeRepose settingAutoUpdateTimeRepose = await settingService.GetSettingAutoUpdateTimeAsync();
                    BackgroundTaskExecute backgroundTaskExecute = new BackgroundTaskExecute();
                    string taskName = "ZXJUpdateTile";
                    string taskEntryPoint = "Weather.Tasks.UpdateTileTask";
                    int time = settingAutoUpdateTimeRepose.AutoUpdateTimes.FirstOrDefault(x => x.Id == int.Parse(userRespose.UserConfig.AutoUpdateTime)).Time;
                    if (BackgroundTaskHelper.IsExist(taskName))
                    {
                        backgroundTaskExecute.Execute(taskName);
                    }
                    else
                    {
                        backgroundTaskExecute.Create(taskName, taskEntryPoint, time, null);
                    }
                } 
                #endregion
            }
            else
            {

            }

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

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void abbRefresh_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 天气指数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void abbWeatherIndex_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 常用城市
        /// /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void abbCity_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MyCityPage));
        }




        /// <summary>
        /// 滚动到视图中后，为第二个数据透视项加载内容。
        /// </summary>
        private async void SecondPivot_Loaded(object sender, RoutedEventArgs e)
        {
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-2");
            this.DefaultViewModel[SecondGroupName] = sampleDataGroup;
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion



    }
}
