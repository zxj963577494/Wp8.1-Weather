using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 天气种类
    /// </summary>
    public class WeatherType
    {
        /// <summary>
        /// 天气唯一标识
        /// </summary>
        public string Wid { get; set; }

        /// <summary>
        /// 天气
        /// </summary>
        public string Weather { get; set; }

        /// <summary>
        /// 今日白天天气图片
        /// </summary>
        public string TodayPic { get; set; }

        /// <summary>
        /// 今日黑夜天气图片
        /// </summary>
        public string TodayNightPic { get; set; }

        /// <summary>
        /// 未来白天天气图片
        /// </summary>
        public string TomorrowPic { get; set; }

        /// <summary>
        /// 未来黑夜天气图片
        /// </summary>
        public string TomorrowNightPic { get; set; }


        /// <summary>
        /// 背景图片
        /// </summary>
        public string BackgroundPic { get; set; }
    }
}
