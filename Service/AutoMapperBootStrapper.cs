using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;

namespace Weather.Service
{
    /// <summary>
    /// 领域模式--视图模型转换类
    /// </summary>
    public class AutoMapperBootStrapper
    {

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Model.WeatherCity, ViewModel.CityView>();
            Mapper.CreateMap<Model.UserConfig, ViewModel.UserConfigForIndexView>();


            Mapper.CreateMap<Model.Sk, ViewModel.WeatherSkView>()
                .ForMember(s => s.time, k => k.MapFrom(src => src.time + "发布"))
                .ForMember(s => s.temp, k => k.MapFrom(src => src.temp + "°"));

            Mapper.CreateMap<Model.Today, ViewModel.WeatherTodayView>()
                .ForMember(t => t.weather_id, k => k.MapFrom(src => src.weather_id.fa)); ;

            Mapper.CreateMap<Model.Future, ViewModel.WeatherFutureView>()
                .ForMember(f => f.weather_id, k => k.MapFrom(src => src.weather_id.fa));

            Mapper.CreateMap<Model.WeatherType, ViewModel.WeatherTypeView>();
               
        }
    }
}
