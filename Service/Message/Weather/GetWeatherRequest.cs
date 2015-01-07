using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    public class GetWeatherRequest
    {
        #region Field
        private string _format = "1";

        private string _dtype = "json";

        private string _key = "4c8397f935951e41ff8a682c50df1690";

        /// <summary>
        /// 城市名称
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

        public GetWeatherRequest()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityname">城市名称</param>
        public GetWeatherRequest(string cityname)
        {
            this.cityname = cityname;
        }

        /// <summary>
        /// 获取请求Url
        /// </summary>
        /// <returns></returns>
        public string GetRequestUrl()
        {
            string url = "http://op.juhe.cn/onebox/weather/query?dtype=json&format=1&cityname=" + Utils.StringHelper.GetUrlString(cityname) + "&key=" + key;
            return url;
        }
        #endregion
    }
}
