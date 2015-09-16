using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 支持城市
    /// </summary>
    public class WeatherCity
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区县（中文）
        /// </summary>
        public string DistrictZH { get; set; }

        /// <summary>
        /// 区县（英文文）
        /// </summary>
        public string DistrictEN { get; set; }
    }
}
