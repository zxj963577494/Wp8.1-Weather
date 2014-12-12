using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 当前实况天气
    /// </summary>
    public class Sk
    {
        /// <summary>
        /// 当前温度 [21]
        /// </summary>
        public string temp { get; set; }

        /// <summary>
        /// 当前风向 [西风]
        /// </summary>
        public string wind_direction { get; set; }

        /// <summary>
        /// 当前风力 [2级]
        /// </summary>
        public string wind_strength { get; set; }

        /// <summary>
        /// 当前湿度 [4%]
        /// </summary>
        public string humidity { get; set; }

        /// <summary>
        /// 更新时间 [14:25]
        /// </summary>
        public string time { get; set; }
    }
}
