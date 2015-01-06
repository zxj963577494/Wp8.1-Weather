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




        public GetSettingAutoUpdateTimeRepose GetSettingAutoUpdateTime()
        {
            GetSettingAutoUpdateTimeRepose respose = new GetSettingAutoUpdateTimeRepose();
            List<Model.AutoUpdateTime> list = new List<Model.AutoUpdateTime>();
            list.Add(new Model.AutoUpdateTime() { Id = 1, Content = "1小时", Time=60 });
            list.Add(new Model.AutoUpdateTime() { Id = 2, Content = "2小时", Time = 120 });
            list.Add(new Model.AutoUpdateTime() { Id = 3, Content = "4小时", Time = 240 });
            list.Add(new Model.AutoUpdateTime() { Id = 4, Content = "6小时", Time = 360 });
            list.Add(new Model.AutoUpdateTime() { Id = 5, Content = "24小时", Time = 1440 });

            respose.AutoUpdateTimes = list;
            return respose;
        }

        public GetSettingSwitchesRespose GetSettingSwitches()
        {
            GetSettingSwitchesRespose respose = new GetSettingSwitchesRespose();
            List<Model.Switchable> list = new List<Model.Switchable>();
            list.Add(new Model.Switchable() { Id = 0, Content = "关" });
            list.Add(new Model.Switchable() { Id = 1, Content = "开" });
            respose.Switches = list;
            return respose;
        }
    }
}
