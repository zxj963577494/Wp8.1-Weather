using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model
{
    /// <summary>
    /// 生活指数
    /// </summary>
    public class Life
    {
        /// <summary>
        /// 公历日期
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// 天气指数信息
        /// </summary>
        public Info info { get; set; }

        public class Info
        {
            /// <summary>
            /// 空调指数["开启制暖空调", "您将感到有些冷，可以适当开启制暖空调调节室内温度，以免着凉感冒。"]
            /// </summary>
            public List<string> kongtiao { get; set; }
            /// <summary>
            /// 运动指数["较不宜", "有降水，且风力较强，推荐您在室内进行低强度运动；若坚持户外运动，请注意保暖并携带雨具。"]
            /// </summary>
            public List<string> yundong { get; set; }
            /// <summary>
            /// 紫外线指数["最弱", "属弱紫外线辐射天气，无需特别防护。若长期在户外，建议涂擦SPF在8-12之间的防晒护肤品。"]
            /// </summary>
            public List<string> ziwaixian { get; set; }
            /// <summary>
            /// 感冒指数["易发", "昼夜温差大，风力较强，易发生感冒，请注意适当增减衣服，加强自我防护避免感冒。"]
            /// </summary>
            public List<string> ganmao { get; set; }
            /// <summary>
            /// 洗车指数["不宜", "不宜洗车，未来24小时内有雨，如果在此期间洗车，雨水和路上的泥水可能会再次弄脏您的爱车。"]
            /// </summary>
            public List<string> xiche { get; set; }
            /// <summary>
            /// 污染指数["良", "气象条件有利于空气污染物稀释、扩散和清除，可在室外正常活动。"]
            /// </summary>
            public List<string> wuran { get; set; }

            /// <summary>
            /// 穿衣指数["冷", "天气冷，建议着棉服、羽绒服、皮夹克加羊毛衫等冬季服装。年老体弱者宜着厚棉衣、冬大衣或厚羽绒服。"]
            /// </summary>
            public List<string> chuanyi { get; set; }
        }



    }


}
