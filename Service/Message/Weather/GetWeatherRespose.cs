using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Model.Weather;

namespace Weather.Service.Message
{
    /// <summary>
    /// 天气响应类
    /// </summary>
    public class GetWeatherRespose
    {
        /// <summary>
        /// 和风天气
        /// </summary>
        public List<HeWeatherItem> result { get; set; }
    }
}
