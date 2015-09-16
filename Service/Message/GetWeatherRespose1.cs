using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    public class GetWeatherRespose1
    {
        public IEnumerable<ViewModel.WeatherFutureView> FutureView { get; set; }

        public ViewModel.WeatherSkView SkView { get; set; }

        public ViewModel.WeatherTodayView TodayView { get; set; }

    }
}
