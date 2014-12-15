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

namespace Weather.Tasks
{
    public sealed class UpdateTileTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            UpdateTile();

            //表示完成任务
            _deferral.Complete();
        }


        public async void UpdateTile()
        {
            UserService userService = new UserService();
            GetUserRespose userRespose = await userService.GetUserAsync();
            var cityList = userRespose.UserConfig.UserCities;
            if (NetHelper.IsNetworkAvailable())
            {
                GetWeatherRespose respose = null;
                int i = 0;
                foreach (var item in cityList)
                {
                    if (i == 0)
                    {
                        //respose = await GetUrlRespose(item.CityName) as GetWeatherRespose;
                        //if (respose != null)
                        //{
                        //    UpdateTile(respose);
                        //}

                        IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.City, item.CityName);
                        string requestUrl = request.GetRequestUrl();
                        string resposeString = await Weather.Utils.HttpHelper.GetUrlRespose(requestUrl);
                        respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(resposeString);
                        await FileHelper.CreateFileForFolder("Temp", item.CityName + "_" + DateTime.Now.ToString("yyyyMMdd"), resposeString);
                        UpdateTile(respose);

                    }
                    else
                    {
                        //await GetUrlRespose(item.CityName);
                        IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.City, item.CityName);
                        string requestUrl = request.GetRequestUrl();
                        string resposeString = await Weather.Utils.HttpHelper.GetUrlRespose(requestUrl);
                        respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(resposeString);
                        await FileHelper.CreateFileForFolder("Temp", item.CityName + "_" + DateTime.Now.ToString("yyyyMMdd"), resposeString);
                    }
                }
            }
            else
            {
                int j = 0;
                foreach (var item in cityList)
                {
                    if (j == 0)
                    {
                       UpdateTileForClient(item.CityName);
                    }
                    else
                    {
                       
                    }
                }
                
            }
        }



        //public async Task<Object> GetUrlRespose(string cityName)
        //{
        //    GetWeatherRespose respose = new GetWeatherRespose();
        //    IGetWeatherRequest request = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.City, cityName);
        //    string requestUrl = request.GetRequestUrl();
        //    string resposeString = await Weather.Utils.HttpHelper.GetUrlRespose(requestUrl);
        //    respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(resposeString);
        //    await FileHelper.CreateFileForFolder("Temp", cityName + "_" + DateTime.Now.ToString("yyyyMMdd"), resposeString);
        //    return respose;
        //}

        public void UpdateTile(Object respose)
        {
            GetWeatherRespose res = respose as GetWeatherRespose;
            if (res != null)
            {
                var tileModel = new
                {
                    ImagerSrc = "ms-appx:///Assets/Logo.scale-100.png",
                    TextHeading = res.result.today.city,
                    TextBody1 = res.result.today.temperature,
                    TextBody2 = res.result.today.weather,
                    TextBody3 = res.result.today.wind
                };

                TileHelper.UpdateTileNotifications(tileModel.ImagerSrc, tileModel.TextHeading, tileModel.TextBody1, tileModel.TextBody2, tileModel.TextBody3);
            }
        }


        public async void GetWeatherForFile(string fileName)
        {
            GetWeatherRespose respose = await JsonSerializeHelper.JsonDeSerializeForFile<GetWeatherRespose>(fileName, "Temp");
            UpdateTile(respose);
        }

        public async void UpdateTileForClient(string cityName)
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
    }
}
