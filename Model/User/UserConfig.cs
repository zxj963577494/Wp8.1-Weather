﻿using System;
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
        /// <summary>
        /// 使用WIfi更新
        /// </summary>
        public int IsWifiUpdate { get; set; }

        /// <summary>
        /// 仅更新默认城市
        /// </summary>
        public int IsUpdateForCity { get; set; }

        /// <summary>
        /// 自动更新频率
        /// </summary>
        public int AutoUpdateTime { get; set; }

        /// <summary>
        /// 是否允许自动更新默认城市
        /// </summary>
        public int IsAutoUpdateForCity { get; set; }

        /// <summary>
        /// 是否允许自动更新所有城市
        /// </summary>
        public int IsAutoUpdateForCities { get; set; }

        /// <summary>
        /// 使用Wifi自动更新
        /// </summary>
        public int IsWifiAutoUpdate { get; set; }
    }
}
