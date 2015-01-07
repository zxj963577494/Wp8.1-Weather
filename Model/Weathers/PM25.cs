using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// PM2.5
    /// </summary>
    public class PM25
    {
        public string key { get; set; }

        public int show_desc { get; set; }

        /// <summary>
        /// PM2.5
        /// </summary>
        public Pm25 pm25 { get; set; }

        /// <summary>
        /// 时间[2015年01月06日16时]
        /// </summary>
        public string dateTime { get; set; }

        /// <summary>
        /// 地区[杭州]
        /// </summary>
        public string cityName { get; set; }




        public class Pm25
        {
            public int curPm { get; set; }

            public int pm25 { get; set; }

            public int pm10 { get; set; }

            /// <summary>
            /// 空气质量等级[2]
            /// </summary>
            public int level { get; set; }
            /// <summary>
            /// 空气质量[良]
            /// </summary>
            public string quality { get; set; }
            /// <summary>
            /// 空气描述[今天的空气质量是可以接受的，除少数异常敏感体质的人群外，大家可在户外正常活动。]
            /// </summary>
            public string des { get; set; }
        }
    }
}
