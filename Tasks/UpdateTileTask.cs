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

            //天气类型
            weatherTypeRespose = await weatherService.GetWeatherTypeAsync();

            //默认城市
            var defaultCity = await GetDefaultCity();

            if (defaultCity != null)
            {
                //用户配置
                userRespose = await userService.GetUserAsync();
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
            //表示完成任务
            _deferral.Complete();
        }

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
            string realResposeString = await GetWeatherString(userCity.CityName);

            GetWeatherRespose weatherRespose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(realResposeString);

            UpdateTile(weatherRespose.result.data);

            await CreateFile(userCity.CityId, realResposeString);
        }



        /// <summary>
        /// 获取天气字符串
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        private async Task<string> GetWeatherString(string cityName)
        {
            GetWeatherRequest request = new GetWeatherRequest(cityName);
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl);
            return resposeString;
        }

        /// <summary>
        /// 保存天气数据
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="realResposeString"></param>
        /// <returns></returns>
        private async Task CreateFile(int cityId, string realResposeString)
        {
            string filePath = "Temp\\" + cityId + "_" + StringHelper.GetTodayDateString() + ".json";
            await FileHelper.CreateAndWriteFileAsync(filePath, realResposeString);
        }


        /// <summary>
        /// 更新磁贴
        /// </summary>
        /// <param name="respose"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTile(Model.Data data)
        {
            string quality = null;
            if (data.pm25 != null)
            {
                quality = data.pm25.pm25.quality;
            }
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
               + "<text id='1'>" + data.realtime.weather.temperature + "</text>"
               + "<text id='2'>" + data.realtime.city_name + "</text>"
               + "<text id='3'>" + data.realtime.weather.temperature + " " + data.realtime.weather.info + "</text>"
               + "<text id='4'>" + data.realtime.wind.direct + " " + data.realtime.wind.power + " " + quality + "</text>"
               + "<text id='5'>" + data.realtime.moon + "</text>"
               + "<text id='6'>" + StringHelper.ConvertWeekNum(data.realtime.week) + "</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Wid == data.realtime.weather.img).TileSquarePic + "'/>"
               + "<text id='1'>" + data.realtime.city_name + "</text>"
               + "<text id='2'>" + data.realtime.weather.temperature + "</text>"
               + "<text id='3'>" + data.realtime.weather.info + "</text>"
               + "<text id='4'>" + data.realtime.wind.direct + " " + data.realtime.wind.power + " " + quality + "</text>"
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
            Model.UserCity defaultCity = await GetDefaultCity();
            GetWeatherRespose weatherRespose = await weatherService.GetWeatherByClientAsync(userCity.CityId.ToString());

            if (weatherRespose.result.data.realtime.date == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                UpdateTile(weatherRespose.result.data);
            }
            else
            {
                Model.WeatherList.Weather weather = weatherRespose.result.data.weather.Find(x => x.date == DateTime.Now.ToString("yyyy-MM-dd"));
                UpdateTileByClientForTomorrow(weather, defaultCity.CityName);
            }
        }

        /// <summary>
        /// 通过本地更新磁贴，未来天气
        /// </summary>
        /// <param name="future"></param>
        /// <param name="cityName"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTileByClientForTomorrow(Model.WeatherList.Weather weather, string cityName)
        {
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
               + "<text id='1'>" + weather.info.day[2] + "°</text>"
               + "<text id='2'>" + cityName + "</text>"
               + "<text id='3'>" + weather.info.day[1] + "</text>"
               + "<text id='4'>" + weather.info.day[3] + weather.info.day[4] + "</text>"
               + "<text id='5'>" + weather.info.night + "</text>"
               + "<text id='6'>星期" + weather.week + "</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + weatherTypeRespose.WeatherTypes.Find(x => x.Wid.ToString() == weather.info.day[0]).TileSquarePic + "'/>"
               + "<text id='1'>" + cityName + "</text>"
               + "<text id='2'>" + weather.info.day[2] + "°</text>"
               + "<text id='3'>" + weather.info.day[1] + "</text>"
               + "<text id='4'>" + weather.info.day[3] + weather.info.day[4] + "</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            TileHelper.UpdateTileNotificationsByXml(tileXmlString);
        }

        #endregion
    }
}
