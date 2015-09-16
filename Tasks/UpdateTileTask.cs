// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Threading;
using System.Threading.Tasks;
using Weather.Service.Implementations;
using Weather.Service.Message;
using Weather.Utils;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.Web.Syndication;
using System.Collections.Generic;
using System.Linq;

namespace Weather.Tasks
{
    public sealed class UpdateTileTask : IBackgroundTask
    {
        private UserService userService = null;
        private WeatherService weatherService = null;
        private GetUserRespose userRespose = null;
        private GetUserCityRespose userCityRespose = null;
        private GetWeatherTypeRespose weatherTypeRespose = null;


        public UpdateTileTask()
        {
            userService = UserService.GetInstance();
            weatherService = WeatherService.GetInstance();
            userRespose = new GetUserRespose();
            userCityRespose = new GetUserCityRespose();
            weatherTypeRespose = new GetWeatherTypeRespose();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();
            //用户配置
            userRespose = await userService.GetUserAsync();
            bool IsAutoUpdateTime = false;
            //是否处于停止更新时间
            if (userRespose.UserConfig.IsAutoUpdateTimeSpan == 1)
            {
                IsAutoUpdateTime = IsAutoUpdateByTime();
            }
            if (!IsAutoUpdateTime)
            {
                //天气类型
                weatherTypeRespose = await weatherService.GetWeatherTypeAsync();

                //默认城市
                var defaultCity = await GetDefaultCity();

                if (defaultCity != null)
                {
                    if (userRespose.UserConfig.IsAutoUpdateForCity == 1)
                    {
                        //有网络
                        if (NetHelper.IsNetworkAvailable())
                        {
                            //无论使用移动数据还是WIFI都允许自动更新
                            if (userRespose.UserConfig.IsWifiAutoUpdate == 0)
                            {
                                await SetWeatherByNetTask(defaultCity);
                            }
                            else //使用WIFI更新
                            {
                                if (NetHelper.IsWifiConnection())
                                {
                                    await SetWeatherByNetTask(defaultCity);
                                }
                                else
                                {
                                    await GetWeatherByClientTask(defaultCity);
                                }
                            }
                        }
                        else
                        {
                            await GetWeatherByClientTask(defaultCity);
                        }
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

        #region 获取默认城市

        /// <summary>
        /// 获取默认城市
        /// </summary>
        /// <returns></returns>
        private async Task<Model.UserCity> GetDefaultCity()
        {
            GetUserCityRespose userRespose = await userService.GetUserCityAsync();
            if (userRespose.UserCities != null)
            {
                return userRespose.UserCities.Find(x => x.IsDefault == 1);
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 通过网络获取天气

        /// <summary>
        /// 通过网络获取天气
        /// </summary>
        /// <param name="userCity"></param>
        /// <returns></returns>
        private async Task SetWeatherByNetTask(Model.UserCity userCity)
        {
            string resposeString = await GetWeatherString(userCity.CityId);

            GetWeatherRespose getWeatherRespose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(resposeString);

            UpdateTile(getWeatherRespose);

            await CreateFile(userCity.CityId, resposeString);
        }



        /// <summary>
        /// 获取天气字符串
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        private async Task<string> GetWeatherString(string cityId)
        {
            IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.CityId, cityId);
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl);
            return resposeString.Replace("HeWeather data service 3.0", "result");
        }

        /// <summary>
        /// 保存天气数据
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="realResposeString"></param>
        /// <returns></returns>
        private async Task CreateFile(string cityId, string resposeString)
        {
            string filePath = "Temp\\" + cityId + "_" + StringHelper.GetTodayDateString() + ".json";
            await FileHelper.CreateAndWriteFileAsync(filePath, resposeString);
        }


        /// <summary>
        /// 更新磁贴
        /// </summary>
        /// <param name="weatherRespose"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTile(GetWeatherRespose weatherRespose)
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
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Code == weatherRespose.result.FirstOrDefault().now.cond.code).TileSquarePic + "'/>"
               + "<text id='1'>" + weatherRespose.result.FirstOrDefault().basic.city + "</text>"
               + "<text id='2'>" + weatherRespose.result.FirstOrDefault().now.tmp + "°</text>"
               + "<text id='3'>" + weatherRespose.result.FirstOrDefault().now.cond.txt + "</text>"
               + "<text id='4'>" + weatherRespose.result.FirstOrDefault().now.wind.dir + " " + weatherRespose.result.FirstOrDefault().now.wind.sc + "级</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            TileHelper.UpdateTileNotificationsByXml(tileXmlString);
        }

        #endregion

        #region 通过本地文件获取天气

        /// <summary>
        /// 通过本地文件获取天气
        /// </summary>
        /// <param name="userCity"></param>
        /// <returns></returns>
        private async Task GetWeatherByClientTask(Model.UserCity userCity)
        {
            GetWeatherRespose respose = await weatherService.GetWeatherByClientAsync(userCity.CityId.ToString());

            // 同一天
            if (respose.result.FirstOrDefault().daily_forecast.FirstOrDefault().date == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                UpdateTile(respose);
            }
            else
            {
                UpdateTileByClientForTomorrow(respose.result.FirstOrDefault().daily_forecast.FirstOrDefault(x => x.date == DateTime.Now.ToString("yyyy-MM-dd")), userCity.CityName);

            }
        }

        /// <summary>
        /// 通过本地更新磁贴，未来天气
        /// </summary>
        /// <param name="future"></param>
        /// <param name="cityName"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTileByClientForTomorrow(Model.Weather.Daily_forecastItem daily_forecast, string cityName)
        {
            string tileXmlString = @"<tile>"
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
               + ((DateTime.Compare(DateTime.Now.ToLocalTime(), DateTime.Parse(daily_forecast.astro.sr)) > 0 & DateTime.Compare(DateTime.Now.ToLocalTime(), DateTime.Parse(daily_forecast.astro.ss)) < 0) ? weatherTypeRespose.WeatherTypes.FirstOrDefault(x => x.Code == daily_forecast.cond.code_d).TileSquarePic : weatherTypeRespose.WeatherTypes.FirstOrDefault(x => x.Code == daily_forecast.cond.code_n).TileSquarePic)
               + "'/>"
               + "<text id='1'>" + cityName + "</text>"
               + "<text id='2'>" + daily_forecast.tmp.min + "°~" + daily_forecast.tmp.max + "</text>"
               + "<text id='3'>" + (daily_forecast.cond.code_d == daily_forecast.cond.code_n ? daily_forecast.cond.txt_d : daily_forecast.cond.txt_d + "转" + daily_forecast.cond.txt_n) + "</text>"
               + "<text id='4'>" + daily_forecast.wind.dir + " " + daily_forecast.wind.sc + "级</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            TileHelper.UpdateTileNotificationsByXml(tileXmlString);
        }

        #endregion
    }
}
