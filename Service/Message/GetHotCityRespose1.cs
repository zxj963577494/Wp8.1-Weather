using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    public class GetHotCityRespose1
    {
        public IEnumerable<ViewModel.CityView> Cities { get; set; }
    }
}
