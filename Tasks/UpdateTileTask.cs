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

    /*
     * 在这里特别要注意，异步方法的调用函数需要放入Run的方法中，并且一个异步方法只能有一个异步调用
     * 另外，里面的方法需要是private static.
     */
    public sealed class UpdateTileTask : IBackgroundTask
    {
        private UserService userService = null;
        private WeatherService weatherService = null;
        private GetUserRespose getUserRespose = null;
        private GetUserCityRespose getUserCityRespose = null;
        private GetWeatherRespose getWeatherRespose = null;
        private GetWeatherTypeRespose getWeatherTypeRespose = null;


        public UpdateTileTask()
        {
            userService = UserService.GetInstance();
            weatherService = WeatherService.GetInstance();
            getUserRespose = new GetUserRespose();
            getUserCityRespose = new GetUserCityRespose();
            getWeatherRespose = new GetWeatherRespose();
            getWeatherTypeRespose = new GetWeatherTypeRespose();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            getWeatherTypeRespose = await weatherService.GetWeatherTypeAsync();

            var defaultCity = await GetDefaultCity();

            if (defaultCity != null)
            {
                getUserRespose = await GetUser();

                if (!NetHelper.IsNetworkAvailable())
                {
                    string realResposeString = null;

                    //无论使用移动数据还是WIFI都允许自动更新
                    if (getUserRespose.UserConfig.IsWifiAutoUpdate == 0)
                    {
                        realResposeString = await GetRealResposeString(defaultCity.CityName);
                        getWeatherRespose = GetUrlRespose(defaultCity.CityName, realResposeString);
                        UpdateTile(getWeatherRespose);
                        await CreateFile(defaultCity.CityId.ToString(), realResposeString);
                    }
                    else
                    {
                        if (NetHelper.IsWifiConnection())
                        {
                            realResposeString = await GetRealResposeString(defaultCity.CityName);
                            getWeatherRespose = GetUrlRespose(defaultCity.CityName, realResposeString);
                            UpdateTile(getWeatherRespose);
                            await CreateFile(defaultCity.CityId.ToString(), realResposeString);
                        }
                    }
                }
                else
                {
                    string fileName = await GetClientPath(defaultCity.CityId.ToString());
                    if (fileName != null)
                    {
                        getWeatherRespose = await GetWeatherByFile(fileName);
                        if (getWeatherRespose.result.today.date_y == DateTime.Now.ToString("yyyy年MM月dd日"))
                        {
                            UpdateTile(getWeatherRespose);
                        }
                        else
                        {
                            Model.Future future = getWeatherRespose.result.future.Find(x => x.date == StringHelper.GetTodayDateString());
                            UpdateTileByClientForTomorrow(future, defaultCity.CityName);
                        }
                    }
                }
            }
            //表示完成任务
            _deferral.Complete();
        }


        /// <summary>
        /// 获取用户配置信息
        /// </summary>
        /// <returns></returns>
        private async Task<GetUserRespose> GetUser()
        {
            return await userService.GetUserAsync();
        }


        /// <summary>
        /// 获取默认城市
        /// </summary>
        /// <returns></returns>
        private async Task<Model.UserCity> GetDefaultCity()
        {
            UserService userService = UserService.GetInstance();
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

        /// <summary>
        /// 获取天气字符串
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>

        private async Task<String> GetRealResposeString(string cityName)
        {
            IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.City, cityName);
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl);
            string realResposeString = HttpHelper.ResposeStringReplace(resposeString);
            return realResposeString;
        }

        /// <summary>
        /// 获取天气对象
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="realResposeString"></param>
        /// <returns></returns>
        private GetWeatherRespose GetUrlRespose(string cityName, string realResposeString)
        {
            GetWeatherRespose respose = new GetWeatherRespose();
            respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(realResposeString);
            return respose;
        }

        /// <summary>
        /// 序列化天气对象
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="realResposeString"></param>
        /// <returns></returns>
        private async Task<bool> CreateFile(string cityId, string realResposeString)
        {
            return await FileHelper.CreateFileForFolderAsync("Temp", cityId + "_" + StringHelper.GetTodayDateString() + ".txt", realResposeString);
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


        #region 本地

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
        /// <summary>
        /// 获取可用本地路径
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        private async Task<String> GetClientPath(string cityId)
        {
            string str = null;
            for (int i = 0; i < 3; i++)
            {
                string fileName = cityId + "_" + DateTime.Now.AddDays(-i).ToString("yyyyMMdd") + ".txt";
                string filePath = "Temp\\" + fileName;
                var x = await FileHelper.IsExistFile(filePath);
                if (x)
                {
                    str = fileName;
                    break;
                }
            }
            return str;
        }

        /// <summary>
        /// 反序列天气对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task<GetWeatherRespose> GetWeatherByFile(string fileName)
        {
            GetWeatherRespose respose = await JsonSerializeHelper.JsonDeSerializeForFileAsync<GetWeatherRespose>(fileName, "Temp");
            return respose;
        }
        #endregion
    }
}
