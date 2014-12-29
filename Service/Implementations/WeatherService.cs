﻿using System;
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

        public async Task<GetWeatherRespose> GetWeatherAsync(IGetWeatherRequest request)
        {
            GetWeatherRespose respose = new GetWeatherRespose();
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl).ConfigureAwait(false);
            string realResposeString = HttpHelper.ResposeStringReplace(resposeString);
            respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(realResposeString);
            return respose;
        }

        public async Task<GetWeatherRespose> GetWeatherByClientAsync(string cityId)
        {
            string fileName = null;
            string filePath = null;
            GetWeatherRespose respose = null;
            for (int i = 0; i < 3; i++)
            {
                fileName = cityId + "_" + DateTime.Now.AddDays(-i).ToString("yyyyMMdd") + ".txt";
                filePath = "Temp\\" + fileName;
                bool x = await FileHelper.IsExistFile(filePath).ConfigureAwait(false);
                if (x)
                {
                    respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileAsync<GetWeatherRespose>(fileName, "Temp").ConfigureAwait(false);
                    break;
                }
            }
            return respose;
        }

        public async Task SaveWeather<T>(T target, string cityId)
        {
            string fileName = cityId + "_" + StringHelper.GetTodayDateString() + ".txt";
            await Weather.Utils.JsonSerializeHelper.JsonSerializeForFileAsync(target, fileName, "Temp").ConfigureAwait(false);
        }

        public async Task<GetWeatherTypeRespose> GetWeatherTypeAsync()
        {
            GetWeatherTypeRespose respose = new GetWeatherTypeRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileAsync<GetWeatherTypeRespose>("WeatherTypes.txt", "Data").ConfigureAwait(false);
            return respose;
        }
    }
}
