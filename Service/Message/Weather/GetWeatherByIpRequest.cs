using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    /// <summary>
    /// 根据IP获取天气
    /// </summary>
    public class GetWeatherByIpRequest : IGetWeatherRequest
    {

        #region Field

        private string _key = "f65dfbde990c433da6a56feda554d759";

        /// <summary>
        /// ip地址
        /// </summary>
        public string cityIp { get; set; }


        /// <summary>
        /// key
        /// </summary>
        public string key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
        #endregion

        #region Method


        public GetWeatherByIpRequest(string cityIp)
        {
            this.cityIp = cityIp;
        }

        /// <summary>
        /// 获取请求Url
        /// </summary>
        /// <returns></returns>
        public string GetRequestUrl()
        {
            return "https://api.heweather.com/x3/weather?cityip=" + cityIp + "&key=" + key;
        }
        #endregion
    }
}
