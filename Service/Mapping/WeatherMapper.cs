using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Mapping
{
    public static class WeatherMapper
    {
        public static ViewModel.CommonCityView ConvertToCommonCityView(Model.UserCity city, Model.WeatherType type, Model.Weather weather)
        {
            ViewModel.CommonCityView view = new ViewModel.CommonCityView();
            view.CityId = city.CityId;
            view.CityName = city.CityName;
            view.TodayPic = (weather == null || type == null) ? null : type.TodayPic;
            view.Temp = weather == null ? null : weather.today.temperature;
            return view;
        }

        public static ViewModel.WeatherSkView ConvertToWeatherSkView(this Model.Sk sk)
        {
            return Mapper.Map<Model.Sk, ViewModel.WeatherSkView>(sk);
        }

        public static ViewModel.WeatherTodayView ConvertToWeatherTodayView(this Model.Today today)
        {
            return Mapper.Map<Model.Today, ViewModel.WeatherTodayView>(today);
        }

        public static IEnumerable<ViewModel.WeatherFutureView> ConvertToWeatherFutureView(this IEnumerable<Model.Future> future)
        {
            return Mapper.Map<IEnumerable<Model.Future>, IEnumerable<ViewModel.WeatherFutureView>>(future);
        }

        public static IEnumerable<ViewModel.WeatherTypeView> ConvertToWeatherTypeView(this IEnumerable<Model.WeatherType> type)
        {
            return Mapper.Map<IEnumerable<Model.WeatherType>, IEnumerable<ViewModel.WeatherTypeView>>(type);
        }
    }
}
