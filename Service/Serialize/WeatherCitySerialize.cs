using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Serialize
{
    public class WeatherCitySerialize
    {
        public IEnumerable<Model.WeatherCity> Cities { get; set; }
    }
}
