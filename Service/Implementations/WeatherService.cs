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
            GetWeatherRespose respose = new GetWeatherRespose();
            string fileName = cityId + "_" + DateTime.Now.ToString("yyyyMMdd")+".txt";
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFile<GetWeatherRespose>(fileName,"Temp");
            return respose;
        }

        public async Task<GetWeatherTypeRespose> GetWeatherTypeAsync()
        {
            GetWeatherTypeRespose respose = new GetWeatherTypeRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFile<GetWeatherTypeRespose>("WeatherTypes.txt", "Data");
            return respose;
        }
    }
}
