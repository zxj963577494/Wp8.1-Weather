using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Message;

namespace Weather.Service.Implementations
{
    public class SettingService
    {
        private static readonly SettingService instance = new SettingService();

        private SettingService()
        {
        }

        public static SettingService GetInstance()
        {
            return instance;
        }




        public async Task<GetSettingAutoUpdateTimeRepose> GetSettingAutoUpdateTimeAsync()
        {
            GetSettingAutoUpdateTimeRepose respose = new GetSettingAutoUpdateTimeRepose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileByInstalledLocationAsync<GetSettingAutoUpdateTimeRepose>("Data\\AutoUpdateTimes.json").ConfigureAwait(false);
            return respose;
        }

        public async Task<GetSettingSwitchesRespose> GetSettingSwitchesAsync()
        {
            GetSettingSwitchesRespose respose = new GetSettingSwitchesRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileByInstalledLocationAsync<GetSettingSwitchesRespose>("Data\\Switches.json").ConfigureAwait(false);
            return respose;
        }
    }
}
