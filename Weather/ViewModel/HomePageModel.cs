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
        public Model.WeatherType WeatherType { get; set; }

        public Model.WeatherList.Weather Today { get; set; }

        public List<Model.WeatherList.Weather> WeatherList { get; set; }

        public Model.Realtime Realtime { get; set; }

        public Model.Life Life { get; set; }

        public Model.PM25 PM25 { get; set; }
    }
}
