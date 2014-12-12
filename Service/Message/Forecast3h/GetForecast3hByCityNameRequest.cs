using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message.Forecast3h
{
    /// <summary>
    /// 根据城市名获取城市天气三小时预报请求类
    /// </summary>
    public class GetForecast3hByCityNameRequest
    {

        #region Field
        private string _dtype = "json";

        private string _key = "fbaccffcb1100c884418266f011bf55e";

        /// <summary>
        /// 城市名称
        /// </summary>
        public string cityname { get; set; }

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityname">城市名称</param>
        public GetForecast3hByCityNameRequest(string cityname)
        {
            this.cityname = cityname;
        }

        /// <summary>
        /// 获取请求Url
        /// </summary>
        /// <returns></returns>
        public string GetRequestUrl()
        {
            string url = "http://v.juhe.cn/weather/forecast3h.php?dtype=json&cityname=" + Utils.StringHelper.GetUrlString(cityname) + "&key=" + key;
            return url;
        } 
        #endregion
    }
}
