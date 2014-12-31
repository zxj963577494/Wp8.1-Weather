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
        private GetUserRespose getUserRespose = null;
        private GetUserCityRespose getUserCityRespose = null;
        private GetWeatherTypeRespose getWeatherTypeRespose = null;


        public UpdateTileTask()
        {
            userService = UserService.GetInstance();
            weatherService = WeatherService.GetInstance();
            getUserRespose = new GetUserRespose();
            getUserCityRespose = new GetUserCityRespose();
            getWeatherTypeRespose = new GetWeatherTypeRespose();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            //天气类型
            getWeatherTypeRespose = await weatherService.GetWeatherTypeAsync();

            //默认城市
            var defaultCity = await GetDefaultCity();

            if (defaultCity != null)
            {
                //用户配置
                getUserRespose = await userService.GetUserAsync();

                //有网络
                if (NetHelper.IsNetworkAvailable())
                {
                    //无论使用移动数据还是WIFI都允许自动更新
                    if (getUserRespose.UserConfig.IsWifiAutoUpdate == 0)
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

            GetWeatherRespose getWeatherRespose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(realResposeString);

            UpdateTile(getWeatherRespose);

            await CreateFile(userCity.CityId, realResposeString);
        }



        /// <summary>
        /// 获取天气字符串
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        private async Task<string> GetWeatherString(string cityName)
        {
            IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.City, cityName);
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl);
            string realResposeString = HttpHelper.ResposeStringReplace(resposeString);
            return realResposeString;
        }

        /// <summary>
        /// 保存天气数据
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="realResposeString"></param>
        /// <returns></returns>
        private async Task CreateFile(int cityId, string realResposeString)
        {
            await FileHelper.CreateFileForFolderAsync("Temp", cityId + "_" + StringHelper.GetTodayDateString() + ".txt", realResposeString);
        }


        /// <summary>
        /// 更新磁贴
        /// </summary>
        /// <param name="respose"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTile(GetWeatherRespose respose)
        {
            //          string tileXmlString = "<tile>"
            //   + "<visual version='2'>"
            //   + "<binding template='TileWide310x150PeekImage03' fallback='TileWidePeekImage03'>"
            //   + "<image id='1' src='ms-appx:///" + (getUserRespose.UserConfig.IsTileSquarePic == 1 ? getWeatherTypeRespose.WeatherTypes.Find(x => x.Wid == respose.result.today.weather_id.fa).TileWidePic : "Assets/Logo.png") + "'/>"
            //   + "<text id='1'>" + respose.result.today.city + "\r\n" + respose.result.sk.temp + "°(" + respose.result.today.temperature + ")\r\n" + respose.result.today.weather + "\r\n" + respose.result.sk.wind_direction + " " + respose.result.sk.wind_strength + "</text>"
            //   + "</binding>"
            //+ "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
            //+ "<image id='1' src='ms-appx:///" + (getUserRespose.UserConfig.IsTileSquarePic == 1 ? getWeatherTypeRespose.WeatherTypes.Find(x => x.Wid == respose.result.today.weather_id.fa).TileSquarePic : "Assets/Logo.png") + "'/>"
            //+ "<text id='1'>" + respose.result.sk.temp + "°</text>"
            //+ "<text id='2'>" + respose.result.today.weather + "</text>"
            //+ "<text id='3'>" + respose.result.today.temperature + "</text>"
            //+ "<text id='4'>" + respose.result.sk.wind_direction + " " + respose.result.sk.wind_strength + "</text>"
            //+ "</binding>"
            //+ "</visual>"
            //+ "</tile>";

            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
               + "<text id='1'>" + respose.result.sk.temp + "°</text>"
               + "<text id='2'>" + respose.result.today.city + "</text>"
               + "<text id='3'>" + respose.result.today.weather + "</text>"
               + "<text id='4'>" + respose.result.today.temperature + "</text>"
               + "<text id='5'>" + respose.result.sk.wind_direction + " " + respose.result.sk.wind_strength + "</text>"
               + "<text id='6'>" + respose.result.today.week + "</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + (getUserRespose.UserConfig.IsTileSquarePic == 1 ? getWeatherTypeRespose.WeatherTypes.Find(x => x.Wid == respose.result.today.weather_id.fa).TileSquarePic : "Assets/Logo.png") + "'/>"
               + "<text id='1'>" + respose.result.today.city + "</text>"
               + "<text id='2'>" + respose.result.today.weather + "</text>"
               + "<text id='3'>" + respose.result.sk.temp + "°</text>"
               + "<text id='4'>" + respose.result.sk.wind_direction + " " + respose.result.sk.wind_strength + "</text>"
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

            GetWeatherRespose getWeatherRespose = await weatherService.GetWeatherByClientAsync(userCity.CityId.ToString());

            if (getWeatherRespose.result.today.date_y == DateTime.Now.ToString("yyyy年MM月dd日"))
            {
                UpdateTile(getWeatherRespose);
            }
            else
            {
                Model.Future future = getWeatherRespose.result.future.Find(x => x.date == StringHelper.GetTodayDateString());
                UpdateTileByClientForTomorrow(future, userCity.CityName);

            }
        }

        /// <summary>
        /// 通过本地更新磁贴，未来天气
        /// </summary>
        /// <param name="future"></param>
        /// <param name="cityName"></param>
        /// <param name="getWeatherTypeRespose"></param>
        /// <param name="getUserRespose"></param>
        private void UpdateTileByClientForTomorrow(Model.Future future, string cityName)
        {
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
               + "<text id='1'>暂无</text>"
               + "<text id='2'>" + cityName + "</text>"
               + "<text id='3'>" + future.weather + "</text>"
               + "<text id='4'>" + future.temperature + "</text>"
               + "<text id='5'>" + future.wind + "</text>"
               + "<text id='6'>" + future.week + "</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///" + (getUserRespose.UserConfig.IsTileSquarePic == 1 ? getWeatherTypeRespose.WeatherTypes.Find(x => x.Wid == future.weather_id.fa).TileSquarePic : "Assets/Logo.png") + "'/>"
               + "<text id='1'>" + cityName + "</text>"
               + "<text id='2'>" + future.weather + "</text>"
               + "<text id='3'>" + future.temperature + "</text>"
               + "<text id='4'>" + future.wind + "</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            TileHelper.UpdateTileNotificationsByXml(tileXmlString);
        }

        #endregion
    }
}
