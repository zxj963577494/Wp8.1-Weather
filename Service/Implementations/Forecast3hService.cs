using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Model;
using Weather.Service.Message.Forecast3h;

namespace Service.Implementations
{
    public class Forecast3hService
    {
        public async Task<GetForecast3hByCityNameRepose> GetWeatherAsync(GetForecast3hByCityNameRequest request)
        {
            GetForecast3hByCityNameRepose respose = new GetForecast3hByCityNameRepose();
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlRespose(requestUrl);
            respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetForecast3hByCityNameRepose>(resposeString);
            return respose;
        }
    }
}
