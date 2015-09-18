using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Weather.Utils
{
    public static class HttpHelper
    {
        /// <summary>
        /// 获取返回字符串获取网络的返回的字符串数据
        /// </summary>
        /// <param name="url"></param>
        public async static Task<string> GetUrlResposeAsnyc(string url)
        {
            Uri uri = new Uri(url);
            HttpClient httpClient = new HttpClient();
            string result = await httpClient.GetStringAsync(uri).ConfigureAwait(false);
            return result;
        }
    }
}
