using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 自动更新频率
    /// </summary>
    public class AutoUpdateTime
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int Time { get; set; }
    }
}
