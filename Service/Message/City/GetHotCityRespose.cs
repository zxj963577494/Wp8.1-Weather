﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    public class GetHotCityRespose
    {
        public List<Model.WeatherCity> Cities { get; set; }
    }
}
