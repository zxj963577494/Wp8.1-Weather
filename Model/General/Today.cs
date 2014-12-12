using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 今日天气
    /// </summary>
    public class Today
    {
        /// <summary>
        /// 城市 [天津]
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 日期 [2014年03月21日]
        /// </summary>
        public string date_y { get; set; }

        /// <summary>
        /// 星期 [星期五]
        /// </summary>
        public string week { get; set; }

        /// <summary>
        /// 今日温度 [8℃~20℃]
        /// </summary>
        public string temperature { get; set; }

        /// <summary>
        /// 今日天气 [晴转霾]
        /// </summary>
        public string weather { get; set; }

        /// <summary>
        /// 天气唯一标识
        /// </summary>
        public WeatherId weather_id { get; set; }

        /// <summary>
        /// 风向风力 [西南风微风]
        /// </summary>
        public string wind { get; set; }

        /// <summary>
        /// 穿衣指数 [较冷]
        /// </summary>
        public string dressing_index { get; set; }

        /// <summary>
        /// 穿衣建议 [建议着大衣、呢外套加毛衣、卫衣等服装。]
        /// </summary>
        public string dressing_advice { get; set; }

        /// <summary>
        /// 紫外线强度 [中等]
        /// </summary>
        public string uv_index { get; set; }

        /// <summary>
        /// 舒适度指数 [中等]
        /// </summary>
        public string comfort_index { get; set; }

        /// <summary>
        /// 洗车指数 [较适宜]
        /// </summary>
        public string wash_index { get; set; }

        /// <summary>
        /// 旅游指数 [适宜]
        /// </summary>
        public string travel_index { get; set; }

        /// <summary>
        /// 晨练指数 [较适宜]
        /// </summary>
        public string exercise_index { get; set; }

        /// <summary>
        /// 干燥指数 [中等]
        /// </summary>
        public string drying_index { get; set; }










    }
}
