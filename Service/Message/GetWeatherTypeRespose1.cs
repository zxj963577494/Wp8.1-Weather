﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    public class GetWeatherTypeRespose1
    {
        public IEnumerable<ViewModel.WeatherTypeView> WeatherTypeView { get; set; }
    }
}
