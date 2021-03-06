﻿using Weather.App.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Weather.Service.Implementations;
using Weather.Service.Message;
using Weather.Utils;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;

// “透视应用程序”模板在 http://go.microsoft.com/fwlink/?LinkID=391641 上有介绍

namespace Weather.App
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    public sealed partial class App : Application
    {
        //private TransitionCollection transitions;
        private UserService userService;
        private GetUserRespose userRespose;
        private GetSettingAutoUpdateTimeRepose settingAutoUpdateTimeRepose;
        private SettingService settingService;




        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            userService = UserService.GetInstance();
            settingService = SettingService.GetInstance();

            userRespose = new GetUserRespose();
            settingAutoUpdateTimeRepose = new GetSettingAutoUpdateTimeRepose();

        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 当启动应用程序以打开特定的文件或显示搜索结果等操作时，
        /// 将使用其他入口点。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif


            await InitializeAppConfig();
            CreateUpdateTileTask();
            CreateUpdateSecondaryTileTask();

            Weather.App.Common.StyleSelector.SetStyle();
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态。
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页。
                rootFrame = new Frame();

                // 将框架与 SuspensionManager 键关联。
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // TODO: 将此值更改为适合您的应用程序的缓存大小。
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // 仅当合适时才还原保存的会话状态。
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // 还原状态时出现问题。
                        // 假定没有状态并继续。
                    }
                }
                rootFrame.NavigationFailed += OnNavigationFailed;
                // 将框架放在当前窗口中。
                Window.Current.Content = rootFrame;
            }

            //if (rootFrame.Content == null)
            //{
            //    // 删除用于启动的旋转门导航。
            //    if (rootFrame.ContentTransitions != null)
            //    {
            //        this.transitions = new TransitionCollection();
            //        foreach (var c in rootFrame.ContentTransitions)
            //        {
            //            this.transitions.Add(c);
            //        }
            //    }

            //    rootFrame.ContentTransitions = null;
            //    rootFrame.Navigated += this.RootFrame_FirstNavigated;

            //    // 当导航堆栈尚未还原时，导航到第一页，
            //    // 并通过将所需信息作为导航参数传入来配置
            //    // 新页面。
            //    if (!rootFrame.Navigate(typeof(PivotPage), e.Arguments))
            //    {
            //        throw new Exception("Failed to create initial page");
            //    }
            //}

            rootFrame.Navigate(typeof(PivotPage), e.Arguments);
            // 确保当前窗口处于活动状态。
            Window.Current.Activate();

        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        ///// <summary>
        ///// 启动应用程序后还原内容转换。
        ///// </summary>
        //private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        //{
        //    var rootFrame = sender as Frame;
        //    rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
        //    rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        //}

        /// <summary>
        /// 在将要挂起应用程序执行时调用。    将保存应用程序状态
        /// 将被终止还是恢复的情况下保存应用程序状态，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起的请求的详细信息。</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        #region 初始化配置文件

        private async Task InitializeAppConfig()
        {
            bool x = await FileHelper.IsExistFileAsync("User\\UserConfig.json");
            if (!x)
            {
                string strUserConfig = @"{'UserConfig':{'IsWifiUpdate':'0','IsUpdateForCity':'1','IsAutoUpdateForCities':'0','IsWifiAutoUpdate':'0','AutoUpdateTime':'2','IsAutoUpdateForCity':'1','IsTileSquarePic':'0','IsAutoUpdateTimeSpan':'0','StopAutoUpdateStartTime':'20:00:00','StopAutoUpdateEndTime':'06:00:00'}}";
                await FileHelper.CreateAndWriteFileAsync("User\\UserConfig.json", strUserConfig);

                await FileHelper.CreateFileAsync("User\\UserCities.json");

            }
        }
        #endregion

        #region 创建更新磁贴

        /// <summary>
        /// 创建更新应用磁贴后台任务
        /// </summary>
        private void CreateUpdateTileTask()
        {
            string taskName = "ZXJUpdateTile";
            string taskEntryPoint = "Weather.Tasks.UpdateTileTask";
            CreateTask(taskName, taskEntryPoint);
        }

        /// <summary>
        /// 创建更新辅助磁贴后台任务
        /// </summary>
        private void CreateUpdateSecondaryTileTask()
        {
            string taskName = "ZXJUpdateSecondaryTile";
            string taskEntryPoint = "Weather.Tasks.UpdateSecondaryTileTask";
            CreateTask(taskName, taskEntryPoint);
        }


        /// <summary>
        /// 创建后台任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="taskEntryPoint"></param>
        private async void CreateTask(string taskName, string taskEntryPoint)
        {
            userRespose = await userService.GetUserAsync();

            BackgroundTaskExecute backgroundTaskExecute = new BackgroundTaskExecute();
            if (BackgroundTaskHelper.IsExist(taskName))
            {
                backgroundTaskExecute.Execute(taskName);
            }
            else
            {
                settingAutoUpdateTimeRepose = settingService.GetSettingAutoUpdateTime();
                int time = settingAutoUpdateTimeRepose.AutoUpdateTimes.FirstOrDefault(x => x.Id == userRespose.UserConfig.AutoUpdateTime).Time;
                backgroundTaskExecute.Create(taskName, taskEntryPoint, time, null);
            }
        }

        #endregion

    }
}
