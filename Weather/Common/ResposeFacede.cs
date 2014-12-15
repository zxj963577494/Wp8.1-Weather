using Service.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Implementations;
using Weather.Service.Message;

namespace Weather.App.Common
{
    public class ResposeFacede
    {
        private SettingService settingService = null;
        private WeatherService weatherService = null;
        private Forecast3hService forecast3hService = null;
        private CityService cityService = null;
        private UserService userService = null;


        public ResposeFacede()
        {
            settingService = new SettingService();
            weatherService = new WeatherService();
            forecast3hService = new Forecast3hService();
            cityService = new CityService();
            userService = new UserService();
        }


        public async Task<GetSettingSwitchesRespose> Get()
        {
            GetSettingSwitchesRespose switchesRespose = new GetSettingSwitchesRespose();
            switchesRespose = await settingService.GetSettingSwitchesAsync();
            return switchesRespose;
        }


    }
}
