using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.App.ViewModel
{
    public class AddCityPageView
    {
        public IEnumerable<Service.ViewModel.CityView> Cities { get; set; }

        public IEnumerable<Service.ViewModel.CityView> HotCities { get; set; }
    }
}
