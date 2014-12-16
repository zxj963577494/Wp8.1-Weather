using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Message;

namespace Weather.Service.Implementations
{
    public class WeatherService
    {
        public async Task<GetWeatherRespose> GetWeatherAsync(IGetWeatherRequest request)
        {
            GetWeatherRespose respose = new GetWeatherRespose();
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl);
            respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(resposeString);
            return respose;
        }


    }
}
