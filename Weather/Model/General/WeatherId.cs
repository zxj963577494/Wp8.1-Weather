using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 天气唯一标识
    /// </summary>
    public class WeatherId
    {
        /// <summary>
        /// 天气标识 [00：晴]
        /// </summary>
        public string fa { get; set; }

        /// <summary>
        /// 天气标识 [53：霾 如果fa不等于fb，说明是组合天气]
        /// </summary>
        public string fb { get; set; }

    }
}
