using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Message.General;

namespace Weather.Service.Message.General
{
    /// <summary>
    /// 根据IP获取天气
    /// </summary>
    public class GetWeatherByIpRequest : IGetWeatherRequest
    {

        #region Field
        private string _format = "1";

        private string _dtype = "json";

        private string _key = "fbaccffcb1100c884418266f011bf55e";

        /// <summary>
        /// ip地址
        /// </summary>
        public string ip { get; set; }

        /// <summary>
        /// 未来6天预报(future)两种返回格式，1或2，默认1
        /// </summary>
        public string format
        {
            get
            {
                return _format;
            }
            set
            {
                _format = value;
            }
        }

        /// <summary>
        /// 返回数据格式：json或xml,默认json
        /// </summary>
        public string dtype
        {
            get
            {
                return _dtype;
            }
            set
            {
                _dtype = value;
            }
        }

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

        public GetWeatherByIpRequest()
        {

        }

        public GetWeatherByIpRequest(string ip)
        {
            this.ip = ip;
        }

        /// <summary>
        /// 获取请求Url
        /// </summary>
        /// <returns></returns>
        public string GetRequestUrl()
        {
            string url = "http://v.juhe.cn/weather/ip?dtype=json&format=" + format + "&ip=" + ip + "&key=" + key;
            return url;
        }  
        #endregion
    }
}
