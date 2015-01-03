using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Message;

namespace Weather.Service.Implementations
{
    public class CityService
    {
        public async Task<GetCityRespose> GetCityAsync()
        {
            GetCityRespose respose = new GetCityRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileByInstalledLocationAsync<GetCityRespose>("Data\\Cities.json").ConfigureAwait(false);
            return respose;
        }

        public async Task<GetHotCityRespose> GetHotCityAsync()
        {
            GetHotCityRespose respose = new GetHotCityRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileByInstalledLocationAsync<GetHotCityRespose>("Data\\HotCities.json").ConfigureAwait(false);
            return respose;
        }

        private static readonly CityService instance = new CityService();

        private CityService()
        {
        }

        public static CityService GetInstance()
        {
            return instance;
        }
    }
}
