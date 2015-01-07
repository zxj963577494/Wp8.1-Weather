using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 实况天气
    /// </summary>
    public class Realtime
    {
        /// <summary>
        /// 风况
        /// </summary>
        public Wind wind { get; set; }

        /// <summary>
        /// 更新时间[16:00:00]
        /// </summary>
        public string time { get; set; }

        /// <summary>
        /// 天气信息
        /// </summary>
        public Weather weather { get; set; }

        /// <summary>
        /// 数据更新时间[1420531810]
        /// </summary>
        public string dataUptime { get; set; }

        /// <summary>
        /// 公历日期[2015-01-06]
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// 城市ID[101210101]
        /// </summary>
        public int city_code { get; set; }


        /// <summary>
        /// 城市名称[杭州]
        /// </summary>
        public string city_name { get; set; }

        /// <summary>
        /// 星期[2]
        /// </summary>
        public int week { get; set; }

                /// <summary>
        /// 农历日期[十一月十六]
        /// </summary>
        public string moon { get; set; }

        /// <summary>
        /// 风况
        /// </summary>
        public class Wind
        {
            /// <summary>
            /// 风速[22.0]
            /// </summary>
            public string windspeed { get; set; }
            /// <summary>
            /// 风向[北风]
            /// </summary>
            public string direct { get; set; }
            /// <summary>
            /// 风力[3级]
            /// </summary>
            public string power { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string offset { get; set; }
        }

        /// <summary>
        /// 天气信息
        /// </summary>
        public class Weather
        {
            /// <summary>
            /// 湿度[86]
            /// </summary>
            public int humidity { get; set; }
            /// <summary>
            /// 天气标识[7]
            /// </summary>
            public int img { get; set; }
            /// <summary>
            /// 天气信息[小雨]
            /// </summary>
            public string info { get; set; }

            /// <summary>
            /// 气温[5]
            /// </summary>
            public string temperature { get; set; }
        }
    }
}
