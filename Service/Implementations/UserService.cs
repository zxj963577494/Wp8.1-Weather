using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Message;

namespace Weather.Service.Implementations
{
    public class UserService
    {
        public async Task<GetUserRespose> GetUserAsync()
        {
            GetUserRespose respose = new GetUserRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFile<GetUserRespose>("UserConfig.txt", "User");
            return respose;
        }

        public void SaveUser(GetUserRespose getUserRespose)
        {
            Weather.Utils.JsonSerializeHelper.JsonSerializeForFile<GetUserRespose>(getUserRespose, "UserConfig.txt", "User");
        }

        public async Task<GetUserCityRespose> GetUserCityAsync()
        {
            GetUserCityRespose respose = new GetUserCityRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFile<GetUserCityRespose>("UserCities.txt", "User");
            return respose;
        }

        public void SaveUserCity(GetUserCityRespose getUserCityRespose)
        {
            Weather.Utils.JsonSerializeHelper.JsonSerializeForFile<GetUserCityRespose>(getUserCityRespose, "UserCities.txt", "User");

        }
    }
}
