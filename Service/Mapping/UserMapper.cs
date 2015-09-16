using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Mapping
{
    public static class UserMapper
    {
        public static IEnumerable<ViewModel.UserCityView> ConvertToCityListView(this 
                                               IEnumerable<Model.UserCity> city)
        {
            return Mapper.Map<IEnumerable<Model.UserCity>, IEnumerable<ViewModel.UserCityView>>(city);
        }


        public static ViewModel.UserConfigForIndexView ConvertUserConfigForIndexView(this Model.UserConfig config)
        {
            return Mapper.Map<Model.UserConfig, ViewModel.UserConfigForIndexView>(config);
        }
    }
}
