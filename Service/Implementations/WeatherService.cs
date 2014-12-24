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
        public WeatherService()
        {

        }

        public async Task<GetWeatherRespose> GetWeatherAsync(IGetWeatherRequest request)
        {
            GetWeatherRespose respose = new GetWeatherRespose();
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl);
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
                bool x = await FileHelper.IsExistFile(filePath);
                if (x)
                {
                    respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFile<GetWeatherRespose>(fileName, "Temp");
                    break;
                }
            }
            return respose;
        }

        public void SaveWeather<T>(T target, string cityId)
        {
            string fileName = cityId + "_" + StringHelper.GetTodayDateString() + ".txt";
            Weather.Utils.JsonSerializeHelper.JsonSerializeForFile(target, fileName, "Temp");
        }

        public async Task<GetWeatherTypeRespose> GetWeatherTypeAsync()
        {
            GetWeatherTypeRespose respose = new GetWeatherTypeRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFile<GetWeatherTypeRespose>("WeatherTypes.txt", "Data");
            return respose;
        }
    }
}
