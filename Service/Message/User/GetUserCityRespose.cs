﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    public class GetUserCityRespose
    {
        public List<Model.UserCity> UserCities { get; set; }
    }
}
