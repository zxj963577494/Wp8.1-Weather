using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Message.Setting;

namespace Weather.Service.Implementations
{
    public class SettingService
    {
        public async Task<GetSettingAutoUpdateTimeRepose> GetSettingAutoUpdateTimeAsync()
        {
            GetSettingAutoUpdateTimeRepose respose = new GetSettingAutoUpdateTimeRepose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFile<GetSettingAutoUpdateTimeRepose>("AutoUpdateTimes.txt", "Data");
            return respose;
        }

        public async Task<GetSettingSwitchesRespose> GetSettingSwitchesAsync()
        {
            GetSettingSwitchesRespose respose = new GetSettingSwitchesRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFile<GetSettingSwitchesRespose>("Switches.txt", "Data");
            return respose;
        }
    }
}
