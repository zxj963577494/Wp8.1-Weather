using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 未来天气集合
    /// </summary>
    public class WeatherList
    {
        public class Weather
        {
            /// <summary>
            /// 公历日期[2015-01-06]
            /// </summary>
            public string date { get; set; }
            /// <summary>
            /// 天气信息
            /// </summary>
            public Info info { get; set; }
            /// <summary>
            /// 星期[二]
            /// </summary>
            public string week { get; set; }
            /// <summary>
            /// 农历日期[十一月十六]
            /// </summary>
            public string nongli { get; set; }


            /// <summary>
            /// 天气信息
            /// </summary>
            public class Info
            {
                /// <summary>
                /// 白天["7", "小雨", "9", "北风", "4-5 级", "06:56"]
                /// </summary>
                public List<string> day { get; set; }
                /// <summary>
                /// 夜晚["6", "雨夹雪", "2", "北风", "4-5 级", "17:13"]
                /// </summary>
                public List<string> night { get; set; }
                /// <summary>
                /// 黎明["2","阴","5","东北风","微风","17:17"]
                /// </summary>
                public List<string> dawn { get; set; }
            }
        }
    }
}
