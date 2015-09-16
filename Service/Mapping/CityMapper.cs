using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.ViewModel;

namespace Weather.Service.Mapping
{
    public static class CityMapper
    {
        public static IEnumerable<CityView> ConvertToCityListView(this 
                                               IEnumerable<Model.WeatherCity> city)
        {
            return Mapper.Map<IEnumerable<Model.WeatherCity>, IEnumerable<ViewModel.CityView>>(city);
        }



    }
}
