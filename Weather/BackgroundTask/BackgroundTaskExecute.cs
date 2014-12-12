using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;

namespace Weather.App.BackgroundTask
{
    /// <summary>
    /// 后台
    /// </summary>
    public class BackgroundTaskExecute
    {
        //public const string UpdateForTileBackgroundTaskEntryPoint = "Weather.BackgroundTask.UpdateTileTask";
        //public const string UpdateForTileBackgroundTaskName = "ZXJUpdateForTile";
        //public static bool updateForTilekRegistered = false;

        ///// <summary>
        ///// 检测后台任务是否已存在
        ///// </summary>
        //public void Vlidate()
        //{
        //    updateForTilekRegistered = BackgroundTaskRegistration.AllTasks.Any(x => x.Value.Name == UpdateForTileBackgroundTaskName);
        //}

        //public BackgroundTaskExecute()
        //{
        //    Vlidate();
        //}

        ///// <summary>
        ///// 创建后台任务
        ///// </summary>
        //public async void RegisterBackgroundTask()
        //{
        //    if (!updateForTilekRegistered)
        //    {
        //        var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
        //        if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
        //            backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
        //        {
        //            try
        //            {
        //                IBackgroundTrigger backgroundTrigger = new TimeTrigger(15, false);
        //                var task = BackgroundTaskHelper.RegisterBackgroundTask(UpdateForTileBackgroundTaskEntryPoint,
        //                                                                   UpdateForTileBackgroundTaskName,
        //                                                                   backgroundTrigger,
        //                                                                   null);
        //                await task;
        //                AttachProgressAndCompletedHandlers(task.Result);
        //            }
        //            catch (Exception)
        //            {

        //                throw;
        //            }

        //            //UpdateUI();
        //        }
        //    }
        //}

        //private void AttachProgressAndCompletedHandlers(IBackgroundTaskRegistration task)
        //{
        //    task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
        //    task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
        //}

        ///// <summary>
        ///// Handle background task progress.
        ///// </summary>
        ///// <param name="task">The task that is reporting progress.</param>
        ///// <param name="e">Arguments of the progress report.</param>
        //private void OnProgress(IBackgroundTaskRegistration task, BackgroundTaskProgressEventArgs args)
        //{
        //    //UpdateUI();
        //}

        ///// <summary>
        ///// Handle background task completion.
        ///// </summary>
        ///// <param name="task">The task that is reporting completion.</param>
        ///// <param name="e">Arguments of the completion report.</param>
        //private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        //{
        //    //UpdateUI();
        //}

        ///// <summary>
        ///// Update the scenario UI.
        ///// </summary>
        //private void UpdateUI()
        //{
        //    //await Depe Dispatcher.RunAsync(CoreDispatcherPriority.Normal);
        //}

        ///// <summary>
        ///// 移除后台任务
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void UnregisterBackgroundTask()
        //{
        //    BackgroundTaskHelper.UnregisterBackgroundTasks(UpdateForTileBackgroundTaskName);
        //    //UpdateBackgroundTaskStatus(UpdateBackgroundTaskStatus, false);

        //    //UpdateUI();
        //}

        //private void UpdateBackgroundTaskStatus(string taskName, bool status)
        //{
        //    switch (taskName)
        //    {
        //        case UpdateForTileBackgroundTaskName:
        //            updateForTilekRegistered = status;
        //            break;
        //        default:
        //            break;
        //    }
        //}


        public void Crete(string backgroundTaskName,string backgroundTaskEntryPoint,IBackgroundTrigger backgroundTrigger, IBackgroundCondition backgroundCondition)
        {
            if (!BackgroundTaskHelper.IsExist(backgroundTaskName))
            {
                BackgroundTaskModel model = new BackgroundTaskModel(backgroundTaskName, backgroundTaskEntryPoint, backgroundTrigger, backgroundCondition);
                model.RegisterBackgroundTask();
            }
        }
    }

    public class BackgroundTaskModel
    {
        public string BackgroundTaskEntryPoint { get; set; }

        public string BackgroundTaskName { get; set; }

        public IBackgroundTrigger BackgroundTrigger { get; set; }

        public IBackgroundCondition BackgroundCondition { get; set; }

        public BackgroundTaskModel()
        {

        }
        public BackgroundTaskModel(string backgroundTaskName, string backgroundTaskEntryPoint, IBackgroundTrigger backgroundTrigger, IBackgroundCondition backgroundCondition)
        {
            this.BackgroundTaskName = backgroundTaskName;
            this.BackgroundTaskEntryPoint = backgroundTaskEntryPoint;
            this.BackgroundTrigger = backgroundTrigger;
            this.BackgroundCondition = backgroundCondition;

        }

        /// <summary>
        /// 创建后台任务
        /// </summary>
        public async void RegisterBackgroundTask()
        {

            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                try
                {
                    var task = BackgroundTaskHelper.RegisterBackgroundTask(this.BackgroundTaskEntryPoint,
                                                                       this.BackgroundTaskName,
                                                                       this.BackgroundTrigger,
                                                                       this.BackgroundCondition);
                    await task;
                    AttachProgressAndCompletedHandlers(task.Result);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private void AttachProgressAndCompletedHandlers(IBackgroundTaskRegistration task)
        {
            task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
            task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
        }

        /// <summary>
        /// Handle background task progress.
        /// </summary>
        /// <param name="task">The task that is reporting progress.</param>
        /// <param name="e">Arguments of the progress report.</param>
        private void OnProgress(IBackgroundTaskRegistration task, BackgroundTaskProgressEventArgs args)
        {
            //UpdateUI();
        }

        /// <summary>
        /// Handle background task completion.
        /// </summary>
        /// <param name="task">The task that is reporting completion.</param>
        /// <param name="e">Arguments of the completion report.</param>
        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            //UpdateUI();
        }
    }
}
