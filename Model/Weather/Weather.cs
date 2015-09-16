using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Model.Weather
{
    /// <summary>
    /// 和风天气
    /// </summary>
    public class HeWeatherItem
    {
        /// <summary>
        /// 空气质量指数
        /// </summary>
        public Aqi aqi { get; set; }

        /// <summary>
        /// 城市基本信息
        /// </summary>
        public Basic basic { get; set; }

        /// <summary>
        /// 天气预报（天）
        /// </summary>
        public List<Daily_forecastItem> daily_forecast { get; set; }

        /// <summary>
        /// 天气预报（小时）
        /// </summary>
        public List<Hourly_forecastItem> hourly_forecast { get; set; }

        /// <summary>
        /// 实况天气
        /// </summary>
        public Now now { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 生活指数
        /// </summary>
        public Suggestion suggestion { get; set; }
    }

    /// <summary>
    /// 空气质量指数
    /// </summary>
    public class Aqi
    {
        /// <summary>
        /// 城市数据
        /// </summary>
        public City city { get; set; }

        /// <summary>
        /// 城市数据
        /// </summary>
        public class City
        {
            /// <summary>
            /// 空气质量指数
            /// </summary>
            public string aqi { get; set; }

            /// <summary>
            /// 一氧化碳1小时平均值(ug/m³)
            /// </summary>
            public string co { get; set; }

            /// <summary>
            /// 二氧化氮1小时平均值(ug/m³)
            /// </summary>
            public string no2 { get; set; }

            /// <summary>
            /// 臭氧1小时平均值(ug/m³)
            /// </summary>
            public string o3 { get; set; }

            /// <summary>
            /// PM10 1小时平均值(ug/m³)
            /// </summary>
            public string pm10 { get; set; }

            /// <summary>
            /// PM2.5 1小时平均值(ug/m³)
            /// </summary>
            public string pm25 { get; set; }

            /// <summary>
            /// 空气质量类别
            /// </summary>
            public string qlty { get; set; }

            /// <summary>
            /// 二氧化硫1小时平均值(ug/m³)
            /// </summary>
            public string so2 { get; set; }

        }
    }

    /// <summary>
    /// 城市基本信息
    /// </summary>
    public class Basic
    {
        /// <summary>
        /// 城市名称
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string cnty { get; set; }

        /// <summary>
        /// 城市ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string lon { get; set; }

        /// <summary>
        /// 数据更新时间,24小时制
        /// </summary>
        public Update update { get; set; }

        /// <summary>
        /// 数据更新时间,24小时制
        /// </summary>
        public class Update
        {
            /// <summary>
            /// 数据更新的当地时间
            /// </summary>
            public string loc { get; set; }

            /// <summary>
            /// 数据更新的UTC时间
            /// </summary>
            public string utc { get; set; }

        }
    }

    /// <summary>
    /// 实况天气
    /// </summary>
    public class Now
    {
        /// <summary>
        /// 天气状况
        /// </summary>
        public Cond cond { get; set; }

        /// <summary>
        /// 体感温度
        /// </summary>
        public string fl { get; set; }

        /// <summary>
        /// 湿度(%)
        /// </summary>
        public string hum { get; set; }

        /// <summary>
        /// 降雨量(mm)
        /// </summary>
        public string pcpn { get; set; }

        /// <summary>
        /// 气压
        /// </summary>
        public string pres { get; set; }

        /// <summary>
        /// 当前温度(摄氏度)
        /// </summary>
        public string tmp { get; set; }

        /// <summary>
        /// 能见度(km)
        /// </summary>
        public string vis { get; set; }

        /// <summary>
        /// 风力状况
        /// </summary>
        public Wind wind { get; set; }

        /// <summary>
        /// 天气状况
        /// </summary>
        public class Cond
        {
            /// <summary>
            /// 天气代码
            /// </summary>
            public string code { get; set; }

            /// <summary>
            /// 天气描述
            /// </summary>
            public string txt { get; set; }

        }


        /// <summary>
        /// 风力状况
        /// </summary>
        public class Wind
        {
            /// <summary>
            /// 风向(角度)
            /// </summary>
            public string deg { get; set; }

            /// <summary>
            /// 风向(方向)
            /// </summary>
            public string dir { get; set; }

            /// <summary>
            /// 风力等级
            /// </summary>
            public string sc { get; set; }

            /// <summary>
            /// 风速(Kmph)
            /// </summary>
            public string spd { get; set; }

        }

    }

    /// <summary>
    /// 天气预报（天）
    /// </summary>
    public class Daily_forecastItem
    {
        /// <summary>
        /// 天文数值
        /// </summary>
        public Astro astro { get; set; }

        /// <summary>
        /// 天气状况
        /// </summary>
        public Cond cond { get; set; }

        /// <summary>
        /// 当地日期
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// 湿度(%)
        /// </summary>
        public string hum { get; set; }

        /// <summary>
        /// 降雨量(mm)
        /// </summary>
        public string pcpn { get; set; }

        /// <summary>
        /// 降水概率
        /// </summary>
        public string pop { get; set; }

        /// <summary>
        /// 气压
        /// </summary>
        public string pres { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public Tmp tmp { get; set; }

        /// <summary>
        /// 能见度(km)
        /// </summary>
        public string vis { get; set; }

        /// <summary>
        /// 风力状况
        /// </summary>
        public Wind wind { get; set; }

        /// <summary>
        /// 天文数值
        /// </summary>
        public class Astro
        {
            /// <summary>
            /// 日出时间
            /// </summary>
            public string sr { get; set; }

            /// <summary>
            /// 日落时间
            /// </summary>
            public string ss { get; set; }

        }


        /// <summary>
        /// 天气状况
        /// </summary>
        public class Cond
        {
            /// <summary>
            /// 白天天气代码
            /// </summary>
            public string code_d { get; set; }

            /// <summary>
            /// 夜间天气代码
            /// </summary>
            public string code_n { get; set; }

            /// <summary>
            /// 白天天气描述
            /// </summary>
            public string txt_d { get; set; }

            /// <summary>
            /// 夜间天气描述
            /// </summary>
            public string txt_n { get; set; }

        }


        /// <summary>
        /// 温度
        /// </summary>
        public class Tmp
        {
            /// <summary>
            /// 最高温度(摄氏度)
            /// </summary>
            public string max { get; set; }

            /// <summary>
            /// 最低温度(摄氏度)
            /// </summary>
            public string min { get; set; }

        }

        /// <summary>
        /// 风力状况
        /// </summary>
        public class Wind
        {
            /// <summary>
            /// 风向(角度)
            /// </summary>
            public string deg { get; set; }

            /// <summary>
            /// 风向(方向)
            /// </summary>
            public string dir { get; set; }

            /// <summary>
            /// 风力等级
            /// </summary>
            public string sc { get; set; }

            /// <summary>
            /// 风速(Kmph)
            /// </summary>
            public string spd { get; set; }

        }

    }

    /// <summary>
    /// 天气预报（小时）
    /// </summary>

    public class Hourly_forecastItem
    {
        /// <summary>
        /// 当地日期和时间
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// 湿度(%)
        /// </summary>
        public string hum { get; set; }

        /// <summary>
        /// 降水概率
        /// </summary>
        public string pop { get; set; }

        /// <summary>
        /// 气压
        /// </summary>
        public string pres { get; set; }

        /// <summary>
        /// 当前温度(摄氏度)
        /// </summary>
        public string tmp { get; set; }

        /// <summary>
        /// 风力状况
        /// </summary>
        public Wind wind { get; set; }

        public class Wind
        {
            /// <summary>
            /// 风向(角度)
            /// </summary>
            public string deg { get; set; }

            /// <summary>
            /// 风向(方向)
            /// </summary>
            public string dir { get; set; }

            /// <summary>
            /// 风力等级
            /// </summary>
            public string sc { get; set; }

            /// <summary>
            /// 风速(Kmph)
            /// </summary>
            public string spd { get; set; }

        }

    }

    /// <summary>
    /// 生活指数
    /// </summary>
    public class Suggestion
    {
        /// <summary>
        /// 舒适度指数
        /// </summary>
        public Comf comf { get; set; }

        /// <summary>
        /// 洗车指数
        /// </summary>
        public Cw cw { get; set; }

        /// <summary>
        /// 穿衣指数
        /// </summary>
        public Drsg drsg { get; set; }

        /// <summary>
        /// 感冒指数
        /// </summary>
        public Flu flu { get; set; }

        /// <summary>
        /// 运动指数
        /// </summary>
        public Sport sport { get; set; }

        /// <summary>
        /// 旅游指数
        /// </summary>
        public Trav trav { get; set; }

        /// <summary>
        /// 紫外线指数
        /// </summary>
        public Uv uv { get; set; }

        /// <summary>
        /// 舒适度指数
        /// </summary>
        public class Comf
        {
            /// <summary>
            /// 简介[舒适]
            /// </summary>
            public string brf { get; set; }

            /// <summary>
            /// 详情[白天不太热也不太冷，风力不大，相信您在这样的天气条件下，应会感到比较清爽和舒适。]
            /// </summary>
            public string txt { get; set; }

        }


        /// <summary>
        /// 
        /// </summary>
        public class Cw
        {
            /// <summary>
            /// 简介[不宜]
            /// </summary>
            public string brf { get; set; }

            /// <summary>
            /// 详情[不宜洗车，未来24小时内有雨，如果在此期间洗车，雨水和路上的泥水可能会再次弄脏您的爱车。]
            /// </summary>
            public string txt { get; set; }

        }


        /// <summary>
        /// 穿衣指数
        /// </summary>
        public class Drsg
        {
            /// <summary>
            /// 简介[舒适]
            /// </summary>
            public string brf { get; set; }

            /// <summary>
            /// 详情[建议着长袖T恤、衬衫加单裤等服装。年老体弱者宜着针织长袖衬衫、马甲和长裤。]
            /// </summary>
            public string txt { get; set; }

        }


        /// <summary>
        /// 感冒指数
        /// </summary>
        public class Flu
        {
            /// <summary>
            /// 简介[少发]
            /// </summary>
            public string brf { get; set; }

            /// <summary>
            /// 详情各项气象条件适宜，无明显降温过程，发生感冒机率较低。]
            /// </summary>
            public string txt { get; set; }

        }


        /// <summary>
        /// 运动指数
        /// </summary>
        public class Sport
        {
            /// <summary>
            /// 简介[较不宜]
            /// </summary>
            public string brf { get; set; }

            /// <summary>
            /// 详情[有降水，推荐您在室内进行健身休闲运动；若坚持户外运动，须注意携带雨具并注意避雨防滑。]
            /// </summary>
            public string txt { get; set; }

        }


        /// <summary>
        /// 旅游指数
        /// </summary>
        public class Trav
        {
            /// <summary>
            /// 简介[适宜]
            /// </summary>
            public string brf { get; set; }

            /// <summary>
            /// 详情[温度适宜，又有较弱降水和微风作伴，会给您的旅行带来意想不到的景象，适宜旅游，可不要错过机会呦！]
            /// </summary>
            public string txt { get; set; }

        }


        /// <summary>
        /// 紫外线指数
        /// </summary>
        public class Uv
        {
            /// <summary>
            /// 简介[最弱]
            /// </summary>
            public string brf { get; set; }

            /// <summary>
            /// 详情[属弱紫外线辐射天气，无需特别防护。若长期在户外，建议涂擦SPF在8-12之间的防晒护肤品。]
            /// </summary>
            public string txt { get; set; }

        }
    }





}
