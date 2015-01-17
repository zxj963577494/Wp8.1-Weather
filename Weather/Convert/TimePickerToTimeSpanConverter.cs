using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Weather.App.Convert
{
    public class TimePickerToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var ss = value.ToString().Split(':');
            TimeSpan ts = new TimeSpan(int.Parse(ss[0]), int.Parse(ss[1]), int.Parse(ss[2]));
            return ts;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
