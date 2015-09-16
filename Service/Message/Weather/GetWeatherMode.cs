using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    /// <summary>
    /// 获取天气的方式
    /// </summary>
    public enum GetWeatherMode
    {
        /// <summary>
        /// 城市名称
        /// </summary>
        City=1,
        /// <summary>
        /// 城市ID
        /// </summary>
        CityId=2,
        /// <summary>
        /// IP地址
        /// </summary>
        CityIp=3,
    }
}
