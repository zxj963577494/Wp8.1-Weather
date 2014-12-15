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
    }
}
