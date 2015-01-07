using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 天气数据
    /// </summary>
    public class Data
    {
        /// <summary>
        /// 实况天气
        /// </summary>
        public Realtime realtime { get; set; }

        /// <summary>
        /// 生活指数
        /// </summary>
        public Life life { get; set; }

        /// <summary>
        /// 未来天气集合
        /// </summary>
        public List<WeatherList.Weather> weather { get; set; }

        /// <summary>
        /// PM2.5
        /// </summary>
        public PM25 pm25 { get; set; }

        public string date { get; set; }
        public int isForeign { get; set; }
    }
}
