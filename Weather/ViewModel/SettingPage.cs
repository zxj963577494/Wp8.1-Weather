using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.App.ViewModel
{
    public class SettingPage
    {
        public AutoUpdateSettingPage AutoUpdateSettingPage { get; set; }

        public GeneralSettingPage GeneralSettingPage { get; set; }

        public Model.UserConfig UserConfig { get; set; }
    }
}
