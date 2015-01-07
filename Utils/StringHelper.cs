using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Utils
{
    /// <summary>
    ///字符串
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 获取今天日期字符串
        /// </summary>
        /// <returns></returns>
        public static string GetTodayDateString()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 获取今天时间字符串
        /// </summary>
        /// <returns></returns>
        public static string GetTimeString()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="p_Text"></param>
        /// <returns></returns>
        public static string GetUrlString(string p_Text)
        {
            byte[] _Value = Encoding.GetEncoding("utf-8").GetBytes(p_Text);

            return "%" + BitConverter.ToString(_Value).Replace('-', '%');
        }


        /// <summary>
        /// 获取今日天气文件路径
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public static string GetTodayFilePath(int cityId)
        {
            string filePath = "Temp\\" + cityId + "_" + StringHelper.GetTodayDateString() + ".json";
            return filePath;
        }



        public static string ConvertWeek(int i)
        {
            string week = null;
            switch (i)
            {
                case 1:
                    week = "星期一";
                    break;
                case 2:
                    week = "星期二";
                    break;
                case 3:
                    week = "星期三";
                    break;
                case 4:
                    week = "星期四";
                    break;
                case 5:
                    week = "星期五";
                    break;
                case 6:
                    week = "星期六";
                    break;
                case 7:
                    week = "星期天";
                    break;
                default:
                    week = "星期一";
                    break;
            }
            return week;
        }
    }
}
