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

        public UpdateSecondaryTileTask()
        {
            userService = new UserService();
            getUserRespose = new GetUserRespose();
            getUserCityRespose = new GetUserCityRespose();
            getWeatherRespose = new GetWeatherRespose();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            //是否自动更新所有城市
            if (getUserRespose.UserConfig.IsAutoUpdateForCities == 1)
            {
                //网络是否开启
                if (NetHelper.IsNetworkAvailable())
                {
                    string realResposeString = null;

                    getUserRespose = await GetUser();

                    //无论使用移动数据还是WIFI都允许自动更新
                    if (getUserRespose.UserConfig.IsWifiAutoUpdate == 0)
                    {
                        getUserCityRespose = await GetUserCity();
                        //允许自动更新所用城市
                        if (getUserRespose.UserConfig.IsAutoUpdateForCities == 1)
                        {
                            foreach (var item in getUserCityRespose.UserCities)
                            {
                                realResposeString = await GetRealResposeString(item.CityName);

                                getWeatherRespose = GetUrlRespose(item.CityName, realResposeString);

                                UpdateSecondaryTile(item.CityId + "_Weather", getWeatherRespose);

                                await CreateFile(item.CityId, realResposeString);
                            }
                        }
                        else//允许更新默认城市
                        {
                            //获取默认城市
                            Model.UserCity userCity = (from u in getUserCityRespose.UserCities
                                                       where u.IsDefault == 1
                                                       select u).FirstOrDefault();

                            realResposeString = await GetRealResposeString(userCity.CityName);

                            getWeatherRespose = GetUrlRespose(userCity.CityName, realResposeString);

                            string tileId = userCity.CityId + "_Weather";

                            if (userCity != null)
                            {
                                UpdateSecondaryTile(tileId, getWeatherRespose);
                            }

                            await CreateFile(userCity.CityId, realResposeString);

                        }

                    }
                    else
                    {
                        if (NetHelper.IsWifiConnection())
                        {
                            getUserCityRespose = await GetUserCity();
                            //允许自动更新所用城市
                            if (getUserRespose.UserConfig.IsAutoUpdateForCities == 1)
                            {
                                foreach (var item in getUserCityRespose.UserCities)
                                {
                                    realResposeString = await GetRealResposeString(item.CityName);

                                    getWeatherRespose = GetUrlRespose(item.CityName, realResposeString);

                                    UpdateSecondaryTile(item.CityId + "_Weather", getWeatherRespose);

                                    await CreateFile(item.CityId, realResposeString);
                                }
                            }
                            else//允许更新默认城市
                            {
                                //获取默认城市
                                Model.UserCity userCity = (from u in getUserCityRespose.UserCities
                                                           where u.IsDefault == 1
                                                           select u).FirstOrDefault();

                                realResposeString = await GetRealResposeString(userCity.CityName);

                                getWeatherRespose = GetUrlRespose(userCity.CityName, realResposeString);

                                string tileId = userCity.CityId + "_Weather";

                                if (userCity != null)
                                {
                                    UpdateSecondaryTile(tileId, getWeatherRespose);
                                }

                                await CreateFile(userCity.CityId, realResposeString);
                            }
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
        /// 获取常用城市
        /// </summary>
        /// <returns></returns>
        private async Task<GetUserCityRespose> GetUserCity()
        {
            return await userService.GetUserCityAsync();
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="realResposeString"></param>
        /// <returns></returns>
        private static GetWeatherRespose GetUrlRespose(string cityName, string realResposeString)
        {
            GetWeatherRespose respose = new GetWeatherRespose();
            respose = Weather.Utils.JsonSerializeHelper.JsonDeserialize<GetWeatherRespose>(realResposeString);
            return respose;
        }

        private static async Task<bool> CreateFile(int cityId, string realResposeString)
        {
            return await FileHelper.CreateFileForFolder("Temp", cityId.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd")+".txt", realResposeString);
        }


        private void UpdateSecondaryTile(string tileId, GetWeatherRespose respose)
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
            SecondaryTileHelper.UpdateSecondaryTileNotificationsByXml(tileId, tileXmlString);
        }

    }

}
