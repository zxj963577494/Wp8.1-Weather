using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message.Forecast3h
{
    /// <summary>
    /// 根据城市名获取城市天气三小时预报响应类
    /// </summary>
    public class GetForecast3hByCityNameRepose
    {
        public string resultcode { get; set; }

        public string reason { get; set; }

        public List<Model.Forecast3h> result { get; set; }

        public string error_code { get; set; }
    }
}
