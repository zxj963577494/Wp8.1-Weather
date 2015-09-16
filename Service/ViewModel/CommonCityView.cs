using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.ViewModel
{
    /// <summary>
    /// 常用城市
    /// </summary>
    public class CommonCityView
    {
        /// <summary>
        /// ID
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 区县
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 今日天气图片
        /// </summary>
        public string TodayPic { get; set; }

        /// <summary>
        /// 气温
        /// </summary>
        public string Temp { get; set; }
    }
}
