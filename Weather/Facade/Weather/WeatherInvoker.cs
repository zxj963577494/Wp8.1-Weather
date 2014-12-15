using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Implementations;
using Weather.Service.Message;

namespace Weather.App.Facade
{
    public class WeatherInvoker
    {
        private WeatherService weatherService = null;
        public WeatherInvoker()
        {
            weatherService = new WeatherService();
        }

        //public Task<GetWeatherRespose> GetWeatherAsync()
        //{

        //    GetWeatherRespose weatherRespose = new GetWeatherRespose();
        //    weatherRespose = await weatherService.GetWeatherAsync();
        //    return weatherRespose;
        //}
    }
}
