using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Message;
using Weather.Utils;

namespace Weather.Service.Implementations
{
    public class WeatherService
    {
        private static readonly WeatherService instance = new WeatherService();

        private WeatherService()
        {
        }

        public static WeatherService GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// 获取天气
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetWeatherRespose> GetWeatherAsync(IGetWeatherRequest request)
        {
            GetWeatherRespose respose = new GetWeatherRespose();
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl).ConfigureAwait(false);
            respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(resposeString.Replace("HeWeather data service 3.0", "result"));
            return respose;
        }

        /// <summary>
        /// 获取本地临时天气数据
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public async Task<GetWeatherRespose> GetWeatherByClientAsync(string cityId)
        {
            string fileName = null;
            string filePath = null;
            GetWeatherRespose respose = null;
            for (int i = 0 ; i < 3 ; i++)
            {
                fileName = cityId + "_" + DateTime.Now.AddDays(-i).ToString("yyyyMMdd") + ".json";
                filePath = "Temp\\" + fileName;
                bool x = await FileHelper.IsExistFileAsync(filePath).ConfigureAwait(false);
                if (x)
                {
                    respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileAsync<GetWeatherRespose>(filePath).ConfigureAwait(false);
                    break;
                }
            }
            return respose;
        }

        /// <summary>
        /// 将天气存储在本地
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public async Task SaveWeather<T>(T target, string cityId)
        {
            string fileName = cityId + "_" + StringHelper.GetTodayDateString() + ".json";
            string filePath = "Temp\\" + fileName;
            await Weather.Utils.JsonSerializeHelper.JsonSerializeForFileAsync(target, filePath).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取天气类型
        /// </summary>
        /// <returns></returns>
        public async Task<GetWeatherTypeRespose> GetWeatherTypeAsync()
        {
            GetWeatherTypeRespose respose = new GetWeatherTypeRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileByInstalledLocationAsync<GetWeatherTypeRespose>("Data\\WeatherTypes.json").ConfigureAwait(false);
            return respose;
        }
    }
}
