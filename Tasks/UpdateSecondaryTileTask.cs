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

            bool IsAutoUpdateTime = false;

            //是否处于停止更新时间
            if (userRespose.UserConfig.IsAutoUpdateTimeSpan == 1)
            {
                IsAutoUpdateTime = IsAutoUpdateByTime();
            }

            if (!IsAutoUpdateTime)
            {
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
            }
            //表示完成任务
            _deferral.Complete();
        }


        #region 判断是否出于停止更新时间

        /// <summary>
        /// 判断是否出于停止更新时间
        /// </summary>
        /// <returns></returns>
        private bool IsAutoUpdateByTime()
        {
            bool isTrue = true;
            DateTime dateStartTime = DateTime.Parse(userRespose.UserConfig.StopAutoUpdateStartTime);
            DateTime dateEndTime = DateTime.Parse(userRespose.UserConfig.StopAutoUpdateEndTime);

            TimeSpan tsStartTime = dateStartTime.TimeOfDay;
            TimeSpan tsEndTime = dateEndTime.TimeOfDay;
            //判断当前时间是否在工作时间段内
            TimeSpan tsNow = DateTime.Now.TimeOfDay;
            if (tsNow > tsStartTime && tsNow < tsEndTime)
            {
                isTrue = true;
            }
            else
            {
                isTrue = false;
            }

            return isTrue;
        }
        #endregion

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
            string resposeString = await GetRealResposeString(userCity.CityId);
            GetWeatherRespose respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(resposeString);
            string tileId = userCity.CityId + "_Weather";
            if (SecondaryTileHelper.IsExists(tileId))
            {
                UpdateSecondaryTile(respose, tileId);
            }
            await CreateFile(userCity.CityId, resposeString);
        }

        /// <summary>
        ///获取天气字符串
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        private static async Task<String> GetRealResposeString(string cityId)
        {
            IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.CityId, cityId);
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl);
            return resposeString.Replace("HeWeather data service 3.0", "result"); ;
        }

        /// <summary>
        /// 创建天气文件
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="realResposeString"></param>
        /// <returns></returns>
        private static async Task<bool> CreateFile(string cityId, string realResposeString)
        {
            string filePath = "Temp\\" + cityId.ToString() + "_" + StringHelper.GetTodayDateString() + ".json";
            return await FileHelper.CreateAndWriteFileAsync(filePath, realResposeString);
        }

        /// <summary>
        /// 更新辅助磁贴信息
        /// </summary>
        /// <param name="tileId"></param>
        private void UpdateSecondaryTile(GetWeatherRespose respose, string tileId)
        {
            string tileXmlString = @"<tile>"
              + "<visual version='2'>"
              + "<binding template='TileWide310x150PeekImage02' fallback='TileWidePeekImage02'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Code == weatherRespose.result.FirstOrDefault().now.cond.code).TileWidePic + "'/>"
               + "<text id='1'>" + weatherRespose.result.FirstOrDefault().basic.city + "</text>"
               + "<text id='2'>" + weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault().tmp.min + "°~" + weatherRespose.result.FirstOrDefault().daily_forecast.FirstOrDefault().tmp.max + "° " + weatherRespose.result.FirstOrDefault().now.cond.txt + "</text>"
               + "<text id='3'>" + weatherRespose.result.FirstOrDefault().now.wind.dir + " " + weatherRespose.result.FirstOrDefault().now.wind.sc + " 级</text>"
               + "<text id='4'>湿度: " + weatherRespose.result.FirstOrDefault().now.hum + "%</text>"
               + "<text id='5'>能见度: " + weatherRespose.result.FirstOrDefault().now.vis + "km</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Code == respose.result.FirstOrDefault().now.cond.code).TileSquarePic + "'/>"
               + "<text id='1'>" + respose.result.FirstOrDefault().basic.city + "</text>"
               + "<text id='2'>" + respose.result.FirstOrDefault().now.tmp + "°</text>"
               + "<text id='3'>" + respose.result.FirstOrDefault().now.cond.txt + "</text>"
               + "<text id='4'>" + respose.result.FirstOrDefault().now.wind.dir + " " + respose.result.FirstOrDefault().now.wind.sc + "</text>"
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
            if (respose.result.FirstOrDefault().daily_forecast.FirstOrDefault().date == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                UpdateSecondaryTile(respose, tileId);
            }
            else
            {
                UpdateTileByClientForTomorrow(tileId, respose.result.FirstOrDefault().daily_forecast.FirstOrDefault(x => x.date == DateTime.Now.ToString("yyyy-MM-dd")), userCity.CityName);
            }
        }

        /// <summary>
        /// 通过本地更新磁贴，未来天气
        /// </summary>
        /// <param name="future"></param>
        /// <param name="cityName"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTileByClientForTomorrow(string tileId, Model.Weather.Daily_forecastItem daily_forecast, string cityName)
        {
            string tileXmlString = "<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150Text01' fallback='TileWideText01'>"
               + "<text id='1'>" + cityName + "</text>"
               + "<text id='2'>" + daily_forecast.tmp.min + "°~" + daily_forecast.tmp.max + "° " + (daily_forecast.cond.code_d == daily_forecast.cond.code_n ? daily_forecast.cond.txt_d : daily_forecast.cond.txt_d + "转" + daily_forecast.cond.txt_n) + "</text>"
               + "<text id='3'>" + daily_forecast.wind.dir + " " + daily_forecast.wind.sc + " 级</text>"
               + "<text id='4'>湿度: " + daily_forecast.hum + "%</text>"
               + "<text id='5'>能见度: " + daily_forecast.vis + "km</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///"
               + ((DateTime.Compare(DateTime.Now.ToLocalTime(), DateTime.Parse(daily_forecast.astro.sr)) > 1 & DateTime.Compare(DateTime.Now.ToLocalTime(), DateTime.Parse(daily_forecast.astro.ss)) < 0) ? weatherTypeRespose.WeatherTypes.Find(x => x.Code == daily_forecast.cond.code_d).TileSquarePic : weatherTypeRespose.WeatherTypes.Find(x => x.Code == daily_forecast.cond.code_n).TileSquarePic)
               + "'/>"
               + "<text id='1'>" + cityName + "</text>"
               + "<text id='2'>" + daily_forecast.tmp.min + "°~" + daily_forecast.tmp.max + "</text>"
               + "<text id='3'>" + (daily_forecast.cond.code_d == daily_forecast.cond.code_n ? daily_forecast.cond.txt_d : daily_forecast.cond.txt_d + "转" + daily_forecast.cond.txt_n) + "</text>"
               + "<text id='4'>" + daily_forecast.wind.dir + " " + daily_forecast.wind.sc + "</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            SecondaryTileHelper.UpdateSecondaryTileNotificationsByXml(tileId, tileXmlString);

        }

        #endregion
    }

}
