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

    public sealed class UpdateSecondaryTileTask : IBackgroundTask
    {
        private UserService userService = null;
        private WeatherService weatherService = null;
        private GetUserRespose userRespose = null;
        private GetUserCityRespose userCityRespose = null;
        private GetWeatherRespose weatherRespose = null;
        private GetWeatherTypeRespose weatherTypeRespose = null;

        public UpdateSecondaryTileTask()
        {
            userService = UserService.GetInstance();
            weatherService = WeatherService.GetInstance();
            userRespose = new GetUserRespose();
            userCityRespose = new GetUserCityRespose();
            weatherRespose = new GetWeatherRespose();
            weatherTypeRespose = new GetWeatherTypeRespose();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            userRespose = await userService.GetUserAsync();

            userCityRespose = await userService.GetUserCityAsync();

            if (userCityRespose != null && userRespose != null)
            {
                //网络是否开启
                if (NetHelper.IsNetworkAvailable())
                {
                    //无论使用移动数据还是WIFI都允许自动更新
                    if (userRespose.UserConfig.IsWifiAutoUpdate == 0)
                    {
                        await UpdateWeather();
                    }
                    else
                    {
                        if (NetHelper.IsWifiConnection())
                        {
                            await UpdateWeather();
                        }
                        else
                        {
                            await UpdateWeatherByClientTask();
                        }
                    }
                }
                else
                {
                    await UpdateWeatherByClientTask();
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
            if (userRespose.UserConfig.IsAutoUpdateForCities == 1)
            {
                await UpdateAllCity();
            }
            else//允许更新默认城市
            {
                if (userRespose.UserConfig.IsAutoUpdateForCity == 1)
                {
                    //获取默认城市
                    Model.UserCity userCity = (from u in userCityRespose.UserCities
                                               where u.IsDefault == 1
                                               select u).FirstOrDefault();

                    await UpdateCity(userCity);
                }
            }
        }

        /// <summary>
        /// 更新所有城市
        /// </summary>
        /// <returns></returns>
        private async Task UpdateAllCity()
        {
            foreach (var item in userCityRespose.UserCities)
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
            GetWeatherRespose respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(realResposeString);
            string tileId = userCity.CityId + "_Weather";
            if (SecondaryTileHelper.IsExists(tileId))
            {
                UpdateSecondaryTile(tileId,respose.result.data.realtime);
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
            GetWeatherRequest request = new GetWeatherRequest(cityName);
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl);
            return resposeString;
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
        private void UpdateSecondaryTile(string tileId, Model.Realtime realtime)
        {
            string tileXmlString = @"<tile>"
+ "<visual version='2'>"
+ "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
+ "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Wid == realtime.weather.img).TileSquarePic + "'/>"
+ "<text id='1'>" + realtime.weather.temperature + "</text>"
+ "<text id='2'>" + realtime.weather.info + "</text>"
+ "<text id='3'>" + realtime.wind.direct + " " + realtime.wind.power + "</text>"
+ "<text id='4'>" + realtime.moon + "</text>"
+ "</binding>"
+ "</visual>"
+ "</tile>";
            SecondaryTileHelper.UpdateSecondaryTileNotificationsByXml(tileId, tileXmlString);
        }


        #region 通过本地文件获取天气




        /// <summary>
        /// 通过本地更新天气
        /// </summary>
        /// <returns></returns>
        private async Task UpdateWeatherByClientTask()
        {

            //允许自动更新所用城市
            if (userRespose.UserConfig.IsAutoUpdateForCities == 1)
            {
                await UpdateAllCityByClientTask();
            }
            else//允许更新默认城市
            {
                if (userRespose.UserConfig.IsAutoUpdateForCity == 1)
                {
                    //获取默认城市
                    Model.UserCity userCity = (from u in userCityRespose.UserCities
                                               where u.IsDefault == 1
                                               select u).FirstOrDefault();

                    await UpdateCityByClientTask(userCity);
                }
            }
        }


        /// <summary>
        /// 通过本地更新所有城市
        /// </summary>
        /// <returns></returns>
        private async Task UpdateAllCityByClientTask()
        {
            foreach (var item in userCityRespose.UserCities)
            {
                await UpdateCityByClientTask(item);
            }
        }


        /// <summary>
        /// 通过本地文件获取天气
        /// </summary>
        /// <param name="userCity"></param>
        /// <returns></returns>
        private async Task UpdateCityByClientTask(Model.UserCity userCity)
        {
            GetWeatherRespose respose = await weatherService.GetWeatherByClientAsync(userCity.CityId.ToString());
            string tileId = userCity.CityId + "_Weather";
            if (weatherRespose.result.data.realtime.date == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                UpdateSecondaryTile(tileId, respose.result.data.realtime);
            }
            else
            {
                Model.WeatherList.Weather weather = weatherRespose.result.data.weather.Find(x => x.date == DateTime.Now.ToString("yyyy-MM-dd"));
                UpdateTileByClientForTomorrow(tileId, weather);

            }
        }

        /// <summary>
        /// 通过本地更新磁贴，未来天气
        /// </summary>
        /// <param name="future"></param>
        /// <param name="cityName"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTileByClientForTomorrow(string tileId, Model.WeatherList.Weather weather)
        {
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Wid.ToString() == weather.info.day[0]).TileSquarePic + "'/>"
               + "<text id='1'>" + weather.info.day[2] + "</text>"
               + "<text id='2'>" + weather.info.day[1] + "</text>"
               + "<text id='3'>" + weather.info.day[3] + " " + weather.info.day[4] + "</text>"
               + "<text id='4'>" + weather.nongli + "</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            SecondaryTileHelper.UpdateSecondaryTileNotificationsByXml(tileId, tileXmlString);
        }

        #endregion
    }

}
