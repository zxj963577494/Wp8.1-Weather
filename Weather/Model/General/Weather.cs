using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    public class WeatherResult
    {
        /// <summary>
        /// 当前实况天气
        /// </summary>
        public Sk sk { get; set; }

        /// <summary>
        /// 今日天气
        /// </summary>
        public Today today { get; set; }

        /// <summary>
        /// 未来天气集合
        /// </summary>
        public List<Future> future { get; set; }
    }
}
