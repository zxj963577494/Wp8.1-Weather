using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.App.ViewModel
{
    public class AutoUpdateSettingPage
    {
        /// <summary>
        /// 自动频率更新
        /// </summary>
        public List<Model.AutoUpdateTime> AutoUpdateTimes { get; set; }

        /// <summary>
        /// 开关
        /// </summary>
        public List<Model.Switchable> Switches { get; set; }
    }
}
