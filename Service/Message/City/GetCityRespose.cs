using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    /// <summary>
    /// 获取城市响应类
    /// </summary>
    public class GetCityRespose
    {
        public List<Model.WeatherCity> Cities { get; set; }
    }
}
