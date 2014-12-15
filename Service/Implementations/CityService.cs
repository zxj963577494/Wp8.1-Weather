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
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFile<GetCityRespose>("Cities.txt","Data");
            return respose;
        }

        public async Task<GetCityRespose> GetHotCityAsync()
        {
            GetCityRespose respose = new GetCityRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFile<GetCityRespose>( "HotCities.txt","Data");
            return respose;
        }
    }
}
