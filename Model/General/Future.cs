using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 未来天气
    /// </summary>
    public class Future
    {
        /// <summary>
        /// 气温 [28℃~36℃]
        /// </summary>
        public string temperature { get; set; }

        /// <summary>
        /// 天气 [晴转多云]
        /// </summary>
        public string weather { get; set; }

        /// <summary>
        /// 天气唯一标识 []
        /// </summary>
        public WeatherId weather_id { get; set; }


        /// <summary>
        /// 风力 [南风3-4级]
        /// </summary>
        public string wind { get; set; }

        /// <summary>
        /// 星期 [星期一]
        /// </summary>
        public string week { get; set; }

        /// <summary>
        /// 日期 [20140804]
        /// </summary>
        public string date { get; set; }
    }

}
