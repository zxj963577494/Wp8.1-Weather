using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message.General
{
    /// <summary>
    /// 获取天气的方式
    /// </summary>
    public enum GetWeatherMode
    {
        City=1,
        Gps=2,
        Ip=3
    }
}
