using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.App.ViewModel
{
    /// <summary>
    /// 首页视图模型
    /// </summary>
    public class HomePageModel
    {
        public Model.WeatherType WeatherType { get; set; }
        /// <summary>
        /// 黎明
        /// </summary>
        public List<Model.WeatherList.Weather> Dawn { get; set; }

        /// <summary>
        /// 白天
        /// </summary>
        public List<Model.WeatherList.Weather> Day { get; set; }

        /// <summary>
        /// 夜晚
        /// </summary>
        public List<Model.WeatherList.Weather> Night { get; set; }


        public Model.Realtime Realtime { get; set; }

        public Model.PM25 PM25 { get; set; }
    }
}
