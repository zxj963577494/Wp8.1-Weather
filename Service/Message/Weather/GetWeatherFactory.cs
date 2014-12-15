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

        public static IGetWeatherRequest CreateGetWeatherRequest(GetWeatherMode mode,string args)
        {
            switch (mode)
            {
                case GetWeatherMode.City:
                    getWeatherRequest = new GetWeatherByCityNameOrIdRequest(args);
                    break;
                case GetWeatherMode.Gps:
                    getWeatherRequest = new GetWeatherByGpsRequest(args.Split(',')[0],args.Split(',')[1]);
                    break;
                case GetWeatherMode.Ip:
                    getWeatherRequest = new GetWeatherByIpRequest(args);
                    break;
                default:
                    getWeatherRequest = new GetWeatherByCityNameOrIdRequest(args);
                    break;
            }
            return getWeatherRequest;
        }
    }
}
