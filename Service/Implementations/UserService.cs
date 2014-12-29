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

        private static readonly UserService instance = new UserService();

        private UserService()
        {
        }

        public static UserService GetInstance()
        {
            return instance;
        }

        public async Task<GetUserRespose> GetUserAsync()
        {
            GetUserRespose respose = new GetUserRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileAsync<GetUserRespose>("UserConfig.txt", "User").ConfigureAwait(false);
            return respose;
        }

        public async Task SaveUser(GetUserRespose getUserRespose)
        {
            await Weather.Utils.JsonSerializeHelper.JsonSerializeForFileAsync<GetUserRespose>(getUserRespose, "UserConfig.txt", "User").ConfigureAwait(false);
        }

        public async Task<GetUserCityRespose> GetUserCityAsync()
        {
            GetUserCityRespose respose = new GetUserCityRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileAsync<GetUserCityRespose>("UserCities.txt", "User").ConfigureAwait(false);
            return respose;
        }

        public async Task SaveUserCity(GetUserCityRespose getUserCityRespose)
        {
            await Weather.Utils.JsonSerializeHelper.JsonSerializeForFileAsync<GetUserCityRespose>(getUserCityRespose, "UserCities.txt", "User").ConfigureAwait(false);

        }
    }
}
