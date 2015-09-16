using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public static string GetTodayFilePath(string cityId)
        {
            string filePath = "Temp\\" + cityId + "_" + StringHelper.GetTodayDateString() + ".json";
            return filePath;
        }

        /// <summary>
        /// 是否是英文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEN(string str)
        {
            Regex regEnglish = new Regex("^[a-zA-Z]");
            if (regEnglish.IsMatch(str))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
