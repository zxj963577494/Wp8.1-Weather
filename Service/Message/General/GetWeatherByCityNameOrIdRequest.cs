using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Message.General;

namespace Weather.Service.Message.General
{
    /// <summary>
    /// 根据城市名称或城市Id获取天气
    /// </summary>
    public class GetWeatherByCityNameOrIdRequest : IGetWeatherRequest
    {
        #region Field
        private string _format = "1";

        private string _dtype = "json";

        private string _key = "fbaccffcb1100c884418266f011bf55e";

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
        /// 城市名或城市ID
        /// </summary>
        public string cityname { get; set; }

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

        public GetWeatherByCityNameOrIdRequest()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityname">城市名称</param>
        public GetWeatherByCityNameOrIdRequest(string cityname)
        {
            this.cityname = cityname;
        }

        /// <summary>
        /// 获取请求Url
        /// </summary>
        /// <returns></returns>
        public string GetRequestUrl()
        {
            string url = "http://v.juhe.cn/weather/index?dtype=json&format=" + format + "&cityname=" + Utils.StringHelper.GetUrlString(cityname) + "&key=" + key;
            return url;
        }
        #endregion
    }
}
