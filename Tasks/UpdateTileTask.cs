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
        private GetUserRespose getUserRespose = null;
        private GetUserCityRespose getUserCityRespose = null;
        private GetWeatherRespose getWeatherRespose = null;

        public UpdateTileTask()
        {
            userService = new UserService();
            getUserRespose = new GetUserRespose();
            getUserCityRespose = new GetUserCityRespose();
            getWeatherRespose = new GetWeatherRespose();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            var defaultCity = await GetDefaultCity();

            getUserRespose = await GetUser();
            //是否自动更新所有城市
            if (getUserRespose.UserConfig.IsAutoUpdateForCities == 1)
            {
                if (getUserRespose.UserConfig.IsAutoUpdateForCity == 1)
                {
                    if (NetHelper.IsNetworkAvailable())
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
                        string fileName = await GetUpdateTileForClientPath(defaultCity.CityId.ToString());
                        if (fileName != null)
                        {
                            getWeatherRespose = await GetWeatherForFile(fileName);
                            Model.Future future = getWeatherRespose.result.future.Find(x => x.date == DateTime.Now.ToString("yyyyMMdd"));
                            UpdateTileForClient(future,defaultCity.CityName);
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

        private static async Task<Model.UserCity> GetDefaultCity()
        {
            UserService userService = new UserService();
            GetUserCityRespose userRespose = await userService.GetUserCityAsync();
            return userRespose.UserCities.Find(x => x.IsDefault == 1);
        }

        private static async Task<String> GetRealResposeString(string cityName)
        {
            IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.City, cityName);
            string requestUrl = request.GetRequestUrl();
            string resposeString = await Weather.Utils.HttpHelper.GetUrlResposeAsnyc(requestUrl);
            string realResposeString = HttpHelper.ResposeStringReplace(resposeString);
            return realResposeString;
        }

        private static GetWeatherRespose GetUrlRespose(string cityName, string realResposeString)
        {
            GetWeatherRespose respose = new GetWeatherRespose();
            respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(realResposeString);
            return respose;
        }

        private static async Task<bool> CreateFile(string cityId, string realResposeString)
        {
            return await FileHelper.CreateFileForFolder("Temp", cityId + "_" + DateTime.Now.ToString("yyyyMMdd")+".txt", realResposeString);
        }


        private static void UpdateTile(GetWeatherRespose respose)
        {
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
               + "<text id='1'>" + respose.result.sk.temp + "°</text>"
               + "<text id='2'>" + respose.result.today.city + "</text>"
               + "<text id='3'>天气：" + respose.result.today.weather + "</text>"
               + "<text id='4'>温度：" + respose.result.today.temperature + "</text>"
               + "<text id='5'>风力：" + respose.result.sk.wind_direction + " " + respose.result.sk.wind_strength + "</text>"
               + "<text id='6'>" + respose.result.today.week + "</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///Assets/Logo.scale-100.png' alt='alt text'/>"
               + "<text id='1'>" + respose.result.today.city + "</text>"
               + "<text id='2'>天气：" + respose.result.today.weather + "</text>"
               + "<text id='3'>温度：" + respose.result.sk.temp + "°</text>"
               + "<text id='4'>风力：" + respose.result.sk.wind_direction + " " + respose.result.sk.wind_strength + "</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            TileHelper.UpdateTileNotificationsByXml(tileXmlString);
        }


        #region 本地

        private void UpdateTileForClient(Model.Future future, string cityName)
        {
            string tileXmlString = @"<tile>"
               + "<visual version='2'>"
               + "<binding template='TileWide310x150BlockAndText01' fallback='TileWideBlockAndText01'>"
               + "<text id='1'>暂无</text>"
               + "<text id='2'>" + cityName + "</text>"
               + "<text id='3'>天气：" + future.weather + "</text>"
               + "<text id='4'>温度：" + future.temperature + "</text>"
               + "<text id='5'>风力：" + future.wind + "</text>"
               + "<text id='6'>" + future.week + "</text>"
               + "</binding>"
               + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
               + "<image id='1' src='ms-appx:///Assets/Logo.scale-100.png' alt='alt text'/>"
               + "<text id='1'>" + cityName + "</text>"
               + "<text id='2'>天气：" + future.weather + "</text>"
               + "<text id='3'>温度：" + future.temperature + "°</text>"
               + "<text id='4'>风力：" + future.wind + "</text>"
               + "</binding>"
               + "</visual>"
               + "</tile>";
            TileHelper.UpdateTileNotificationsByXml(tileXmlString);
        }

        private async Task<String> GetUpdateTileForClientPath(string cityId)
        {
            string str = null;
            for (int i = 0; i < 7; i--)
            {
                string fileName = cityId + "_" + DateTime.Now.AddDays(i).ToString("yyyyMMdd")+".txt";
                string filePath = "Temp\\" + fileName;
                var file = await FileHelper.GetFileAccess(filePath);
                if (file != null)
                {
                    str = fileName;
                    break;
                }
            }
            return str;
        }

        private async Task<GetWeatherRespose> GetWeatherForFile(string fileName)
        {
            GetWeatherRespose respose = await JsonSerializeHelper.JsonDeSerializeForFile<GetWeatherRespose>(fileName,"Temp");
            return respose;
        } 
        #endregion
    }
}
