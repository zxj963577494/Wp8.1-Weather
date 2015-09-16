using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 用户常用城市
    /// </summary>
    public class UserCity
    {
        /// <summary>
        /// 城市ID
        /// </summary>
        public string CityId { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 是否默认城市
        /// </summary>
        public int IsDefault { get; set; }
    }
}
