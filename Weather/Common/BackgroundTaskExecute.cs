using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Utils;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;

namespace Weather.App.Common
{
    /// <summary>
    /// 后台
    /// </summary>
    public class BackgroundTaskExecute
    {
        public void Execute(string backgroundTaskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == backgroundTaskName)
                {
                    AttachProgressAndCompletedHandlers(task.Value);
                }
            }
        }

        public async void Create(string backgroundTaskName, string backgroundTaskEntryPoint, int time, IBackgroundCondition backgroundCondition)
        {

            IBackgroundTrigger backgroundTrigger = new TimeTrigger((uint)time, false);
            var task = BackgroundTaskHelper.RegisterBackgroundTask(backgroundTaskEntryPoint, backgroundTaskName, backgroundTrigger, backgroundCondition);
            await task;
            AttachProgressAndCompletedHandlers(task.Result);

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
