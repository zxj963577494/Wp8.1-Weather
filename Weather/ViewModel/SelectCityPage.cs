using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.App.ViewModel
{
    /// <summary>
    /// 城市选择页面
    /// </summary>
    public class SelectCityPage
    {
        public List<Model.WeatherCity> Cities { get; set; }

        public List<Model.WeatherCity> HotCities { get; set; }
    }
}
