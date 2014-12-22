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

        public List<Model.Future> Futures { get; set; }

        public Model.Sk Sk { get; set; }

        public Model.Today Today { get; set; }
    }
}
