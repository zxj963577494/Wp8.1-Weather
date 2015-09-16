using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Serialize
{
    public class WeatherSerialize
    {
        public string resultcode { get; set; }

        public string reason { get; set; }

        public Model.Weather result { get; set; }

        public string error_code { get; set; }
    }
}
