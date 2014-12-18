using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Weather.Tasks
{
     /*
     * 在这里特别要注意，异步方法的调用函数需要放入Run的方法中，并且一个异步方法只能有一个异步调用
     * 另外，里面的方法需要是private static.
     */
    public sealed class UpdateSecondaryTileTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            //表示完成任务
            _deferral.Complete();
        }
    }

}
