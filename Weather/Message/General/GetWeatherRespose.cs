using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Message.General
{
    /// <summary>
    /// 天气响应类
    /// </summary>
    public class GetWeatherRespose
    {
        public string resultcode { get; set; }

        public string reason { get; set; }

        public Model.WeatherResult result { get; set; }

        public string error_code { get; set; }
    }
}
