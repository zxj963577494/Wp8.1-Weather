using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    public class GetWeatherRespose
    {
        public string reason { get; set; }

        public Result result { get; set; }

        public string error_code { get; set; }


        public class Result
        {
            public Model.Data data { get; set; }
        }
    }
}
