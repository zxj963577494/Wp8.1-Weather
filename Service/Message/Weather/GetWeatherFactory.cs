using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    /// <summary>
    /// 请求工厂
    /// </summary>
    public class GetWeatherRequestFactory
    {
        private static IGetWeatherRequest getWeatherRequest = null;

        public static IGetWeatherRequest CreateGetWeatherRequest(GetWeatherMode mode, string args)
        {
            switch (mode)
            {
                case GetWeatherMode.City:
                    getWeatherRequest = new GetWeatherByCityNameRequest(args);
                    break;
                case GetWeatherMode.CityId:
                    getWeatherRequest = new GetWeatherByCityIdRequest(args);
                    break;
                case GetWeatherMode.CityIp:
                    getWeatherRequest = new GetWeatherByIpRequest(args);
                    break;
                default:
                    getWeatherRequest = new GetWeatherByCityIdRequest(args);
                    break;
            }
            return getWeatherRequest;
        }
    }
}
