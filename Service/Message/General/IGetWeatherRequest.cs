using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message.General
{
    public interface IGetWeatherRequest
    {
        string GetRequestUrl();
    }
}
