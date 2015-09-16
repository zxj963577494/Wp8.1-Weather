using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.App.ViewModel
{
    /// <summary>
    /// 首页视图模型
    /// </summary>
    public class HomePageModel
    {
        /// <summary>
        /// 当前温度
        /// </summary>
        public string Tmp { get; set; }

        /// <summary>
        /// 最低温度~最高温度
        /// </summary>
        public string Daytmp { get; set; }

        /// <summary>
        /// 风况
        /// </summary>
        public string Wind { get; set; }

        /// <summary>
        /// 空气质量
        /// </summary>
        public string Aqi { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string Update { get; set; }

        /// <summary>
        /// 天气描述
        /// </summary>
        public string Txt { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        public string Hum { get; set; }

        /// <summary>
        /// 气压
        /// </summary>
        public string Pres { get; set; }

        /// <summary>
        /// 可见度
        /// </summary>
        public string Vis { get; set; }

        /// <summary>
        /// 天气类型
        /// </summary>
        public Model.WeatherType WeatherType { get; set; }

        /// <summary>
        /// 天气预报（天）
        /// </summary>
        public List<DailyItem> DailyList { get; set; }

        /// <summary>
        /// 天气预报（小时）
        /// </summary>
        public List<HourlyItem> HourlyList { get; set; }

        /// <summary>
        /// 生活指数
        /// </summary>
        public List<Model.Weather.Suggestion> Suggestion { get; set; }


    }

    /// <summary>
    /// 天气预报（天）
    /// </summary>
    public class DailyItem
    {
        public string Date { get; set; }
        public string Image { get; set; }
        public string Tmp { get; set; }
        public string Txt { get; set; }
    }

    /// <summary>
    /// 天气预报（小时）
    /// </summary>
    public class HourlyItem
    {
        public string Date { get; set; }
        public string Wind { get; set; }
        public string Tmp { get; set; }
        public string Hum { get; set; }
    }
}
