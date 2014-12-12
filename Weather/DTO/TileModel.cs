using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.App.DTO
{
    /// <summary>
    /// 磁铁内容更新传输类
    /// </summary>
    public class TileModel
    {
        /// <summary>
        /// 前景图片路径
        /// </summary>
        public string ImagerSrc { get; set; }

        /// <summary>
        /// 头部文字
        /// </summary>
        public string TextHeading { get; set; }

        /// <summary>
        /// 第一行文字
        /// </summary>
        public string TextBody1 { get; set; }

        /// <summary>
        /// 第二行文字
        /// </summary>
        public string TextBody2 { get; set; }

        /// <summary>
        /// 第三行文字
        /// </summary>
        public string TextBody3 { get; set; }

    }
}
