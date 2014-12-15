using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 用户类
    /// </summary>
    public class UserConfig
    {
        public List<UserCity> UserCities { get; set; }

        /// <summary>
        /// 自动更新频率
        /// </summary>
        public string AutoUpdateTime { get; set; }

        /// <summary>
        /// 是否允许自动更新
        /// </summary>
        public string IsAutoUpdate { get; set; }
    }
}
