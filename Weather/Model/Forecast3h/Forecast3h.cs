using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 城市天气三小时预报
    /// </summary>
    public class Forecast3h
    {
        /// <summary>
        /// 天气标识ID [00]
        /// </summary>
        public string weatherid { get; set; }

        /// <summary>
        /// 天气 [晴]
        /// </summary>
        public string weather { get; set; }

        /// <summary>
        /// 低温 [27]
        /// </summary>
        public string temp1 { get; set; }

        /// <summary>
        /// 高温 [31]
        /// </summary>
        public string temp2 { get; set; }

        /// <summary>
        /// 开始小时 [08]
        /// </summary>
        public string sh { get; set; }

        /// <summary>
        /// 结束小时 [11]
        /// </summary>
        public string eh { get; set; }

        /// <summary>
        /// 日期 [20140530]
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// 完整开始时间 [20140530080000]
        /// </summary>
        public string sfdate { get; set; }

        /// <summary>
        /// 完整结束时间 [20140530110000]
        /// </summary>
        public string efdate { get; set; }
    }
}
