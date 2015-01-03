using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Implementations;
using Weather.Service.Message;
using Weather.Utils;
using Windows.ApplicationModel.Background;

namespace Weather.Tasks
{
    /*
    * 在这里特别要注意，异步方法的调用函数需要放入Run的方法中，并且一个异步方法只能有一个异步调用
    * 另外，里面的方法需要是private static.
    */
    public sealed class UpdateSecondaryTileTask : IBackgroundTask
    {
        private UserService userService = null;
        private GetUserRespose getUserRespose = null;
        private GetUserCityRespose getUserCityRespose = null;
        private GetWeatherRespose getWeatherRespose = null;
        private GetWeatherTypeRespose getWeatherTypeRespose = null;

        public UpdateSecondaryTileTask()
        {
            userService = UserService.GetInstance();
            getUserRespose = new GetUserRespose();
            getUserCityRespose = new GetUserCityRespose();
            getWeatherRespose = new GetWeatherRespose();
            getWeatherTypeRespose = new GetWeatherTypeRespose();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            //网络是否开启
            if (NetHelper.IsNetworkAvailable())
            {
                getUserRespose = await userService.GetUserAsync();

                getUserCityRespose = await userService.GetUserCityAsync();

                if (getUserCityRespose != null && getUserRespose != null)
                {
                    //无论使用移动数据还是WIFI都允许自动更新
                    if (getUserRespose.UserConfig.IsWifiAutoUpdate == 0)
                    {
                        await UpdateWeather();
                    }
                    else
                    {
                        if (NetHelper.IsWifiConnection())
                        {
                            await UpdateWeather();
                        }
                    }
                }
            }
            //表示完成任务
            _deferral.Complete();
        }

        /// <summary>
        /// 更新天气
        /// </summary>
        /// <returns></returns>
        private async Task UpdateWeather()
        {
            //允许自动更新所用城市
            if (getUserRespose.UserConfig.IsAutoUpdateForCities == 1)
            {
                await UpdateAllCity();
            }
            else//允许更新默认城市
            {
                //获取默认城市
                Model.UserCity userCity = (from u in getUserCityRespose.UserCities
                                           where u.IsDefault == 1
                                           select u).FirstOrDefault();

                await UpdateCity(userCity);
            }
        }


        /// <summary>
        /// 更新所有城市
        /// </summary>
        /// <returns></returns>
        private async Task UpdateAllCity()
        {
            foreach (var item in getUserCityRespose.UserCities)
            {
                await UpdateCity(item);
            }
        }



        /// <summary>
        /// 更新单个城市
        /// </summary>
        /// <returns></returns>
        private async Task UpdateCity(Model.UserCity userCity)
        {
            string realResposeString = await GetRealResposeString(userCity.CityName);
            getWeatherRespose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(realResposeString);
            string tileId = userCity.CityId + "_Weather";
            if (SecondaryTileHelper.IsExists(tileId))
            {
                UpdateSecondaryTile(tileId);
            }
            await CreateFile(userCity.CityId, realResposeString);
        }

        /// <summary>
        ///获取天气字符串
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        private static async Task<String> GetRealResposeString(string cityName)
        {
            IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.City, cityName);
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl);
            string realResposeString = HttpHelper.ResposeStringReplace(resposeString);
            return realResposeString;
        }

        /// <summary>
        /// 创建天气文件
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="realResposeString"></param>
        /// <returns></returns>
        private static async Task<bool> CreateFile(int cityId, string realResposeString)
        {
            string filePath = "Temp\\" + cityId.ToString() + "_" + StringHelper.GetTodayDateString() + ".json";
            return await FileHelper.CreateAndWriteFileAsync(filePath, realResposeString);
        }

        /// <summary>
        /// 更新辅助磁贴信息
        /// </summary>
        /// <param name="tileId"></param>
        private void UpdateSecondaryTile(string tileId)
        {
            string tileXmlString = @"<tile>"
              + "<visual version='2'>"
              + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
              + "<text id='1'>" + getWeatherRespose.result.sk.temp + "°</text>"
              + "<text id='2'>" + getWeatherRespose.result.today.city + "</text>"
              + "<text id='3'>" + getWeatherRespose.result.today.weather + "</text>"
              + "<text id='4'>" + getWeatherRespose.result.today.temperature + "</text>"
              + "<text id='5'>" + getWeatherRespose.result.sk.wind_direction + " " + getWeatherRespose.result.sk.wind_strength + "</text>"
              + "<text id='6'>" + getWeatherRespose.result.today.week + "</text>"
              + "</binding>"
              + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
              + "<image id='1' src='ms-appx:///" + getWeatherTypeRespose.WeatherTypes.Find(x => x.Wid == getWeatherRespose.result.today.weather_id.fa).TileSquarePic + "'/>"
              + "<text id='1'>" + getWeatherRespose.result.sk.temp + "°</text>"
              + "<text id='2'>" + getWeatherRespose.result.today.weather + "</text>"
              + "<text id='3'>" + getWeatherRespose.result.today.temperature + "</text>"
              + "<text id='4'>" + getWeatherRespose.result.sk.wind_direction + " " + getWeatherRespose.result.sk.wind_strength + "</text>"
              + "</binding>"
              + "</visual>"
              + "</tile>";
            SecondaryTileHelper.UpdateSecondaryTileNotificationsByXml(tileId, tileXmlString);
        }

    }

}
