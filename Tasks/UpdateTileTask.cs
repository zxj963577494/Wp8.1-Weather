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
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            var cityList = await GetCityList();

            GetWeatherRespose respose = null;
            string realResposeString = null;
            if (NetHelper.IsNetworkAvailable())
            {
                int i = 0;
                foreach (var item in cityList)
                {
                    if (i == 0)
                    {
                        realResposeString = await GetRealResposeString(item.CityName);
                        respose = GetUrlRespose(item.CityName, realResposeString);
                        UpdateTile(respose);
                        await CreateFile(item.CityName, realResposeString);
                    }
                    else
                    {
                        realResposeString = await GetRealResposeString(item.CityName);
                        respose = GetUrlRespose(item.CityName, realResposeString);
                        await CreateFile(item.CityName, realResposeString);
                    }
                    i++;
                }
            }
            else
            {
                int j = 0;
                foreach (var item in cityList)
                {
                    if (j == 0)
                    {
                        respose = await GetWeatherForFile(item.CityName);
                        UpdateTile(respose);
                    }
                    else
                    {

                    }
                    j++;
                }
            }

            //表示完成任务
            _deferral.Complete();
        }

        private static async Task<List<Model.UserCity>> GetCityList()
        {
            UserService userService = new UserService();
            GetUserRespose userRespose = await userService.GetUserAsync();
            var cityList = userRespose.UserConfig.UserCities;
            return cityList;
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

        private static async Task<bool> CreateFile(string cityName, string realResposeString)
        {
            return await FileHelper.CreateFileForFolder("Temp", cityName + "_" + DateTime.Now.ToString("yyyyMMdd"), realResposeString);
        }


        private static void UpdateTile(GetWeatherRespose respose)
        {
            var tileModel = new
            {
                ImagerSrc = "ms-appx:///Assets/Logo.scale-100.png",
                TextHeading = respose.result.today.city,
                TextBody1 = respose.result.today.temperature,
                TextBody2 = respose.result.today.weather,
                TextBody3 = respose.result.today.wind
            };

            TileHelper.UpdateTileNotifications(tileModel.ImagerSrc, tileModel.TextHeading, tileModel.TextBody1, tileModel.TextBody2, tileModel.TextBody3);

        }

        private static async void UpdateTileForClient(string cityName)
        {
            for (int i = 0; i < 7; i++)
            {
                i--;
                string fileName = cityName + "_" + DateTime.Now.AddDays(i).ToString("yyyy-MM-dd");
                bool isExist = await FileHelper.IsExistFile("Temp", fileName);
                if (isExist)
                {
                    GetWeatherForFile(fileName);
                }
            }
        }

        private static async Task<GetWeatherRespose> GetWeatherForFile(string fileName)
        {
            GetWeatherRespose respose = await JsonSerializeHelper.JsonDeSerializeForFile<GetWeatherRespose>(fileName, "Temp");
            return respose;
        }
    }
}
