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



        public static string ConvertWeekNum(int i)
        {
            string week = null;
            switch (i)
            {
                case 1:
                    week = "周一";
                    break;
                case 2:
                    week = "周二";
                    break;
                case 3:
                    week = "周三";
                    break;
                case 4:
                    week = "周四";
                    break;
                case 5:
                    week = "周五";
                    break;
                case 6:
                    week = "周六";
                    break;
                case 7:
                    week = "周日";
                    break;
                default:
                    week = "周一";
                    break;
            }
            return week;
        }

        public static string ConvertWeekCn(string str)
        {
            string week = null;
            switch (str)
            {
                case "一":
                    week = "周一";
                    break;
                case "二":
                    week = "周二";
                    break;
                case "三":
                    week = "周三";
                    break;
                case "四":
                    week = "周四";
                    break;
                case "五":
                    week = "周五";
                    break;
                case "六":
                    week = "周六";
                    break;
                case "日":
                    week = "周日";
                    break;
                default:
                    week = "周一";
                    break;
            }
            return week;
        }
    }
}
