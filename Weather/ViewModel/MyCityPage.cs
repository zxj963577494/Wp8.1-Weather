using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Message;

namespace Weather.App.ViewModel
{
    public class MyCityPage
    {
        public List<MyCityPageModel> MyCityPageModels { get; set; }

    }


    public class MyCityPageModel
    {
        public string CityId { get; set; }

        public string CityName { get; set; }

        public string TodayPic { get; set; }

        public string Temp { get; set; }
    }
}
