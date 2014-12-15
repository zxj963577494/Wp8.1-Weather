using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Weather.Utils
{
    /// <summary>
    /// 后台任务
    /// </summary>
    public static class BackgroundTaskHelper
    {
        /*
         * 在 Windows Phone 上，你必须在尝试注册任何后台任务之前调用 RequestAccessAsync
         */

        /// <summary>
        /// 注册后台任务
        /// </summary>
        /// <param name="taskEntryPoint">任务的入口点</param>
        /// <param name="name">任务名称</param>
        /// <param name="trigger">轮询时间</param>
        /// <param name="condition">系统事件</param>
        /// <returns></returns>
        public static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(String taskEntryPoint, String name, IBackgroundTrigger trigger, IBackgroundCondition condition)
        {
            BackgroundTaskRegistration task = null;
            if (TaskRequiresBackgroundAccess(name))
            {
                await BackgroundExecutionManager.RequestAccessAsync();
            }
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {

                var builder = new BackgroundTaskBuilder();

                builder.Name = name;
                builder.TaskEntryPoint = taskEntryPoint;
                builder.SetTrigger(trigger);

                if (condition != null)
                {
                    builder.AddCondition(condition);
                    builder.CancelOnConditionLoss = true;
                }
                task = builder.Register();
            }
            return task;
        }

        /// <summary>
        ///移除后台任务
        /// </summary>
        /// <param name="name">任务名称</param>
        public static void UnregisterBackgroundTasks(string name)
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name)
                {
                    cur.Value.Unregister(true);
                }
            }
        }

        /// <summary>
        /// 检测后台任务是否已存在
        /// </summary>
        public static bool IsExist(string backgroundTaskName)
        {
            return BackgroundTaskRegistration.AllTasks.Any(x => x.Value.Name == backgroundTaskName);
        }


        /// <summary>
        /// Determine if task with given name requires background access.
        /// </summary>
        /// <param name="name">Name of background task to query background access requirement.</param>
        public static bool TaskRequiresBackgroundAccess(String name)
        {
            return true;
            //#if WINDOWS_PHONE_APP
            //            return true;
            //#else
            //            if (name == TimeTriggeredTaskName)
            //            {
            //                return true;
            //            }
            //            else
            //            {
            //                return false;
            //            }
            //#endif
        }
    }
}
