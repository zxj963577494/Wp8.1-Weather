using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;

namespace Weather.Common
{
    /// <summary>
    /// 位置
    /// </summary>
    public static class GeolocationHelper
    {
        /// <summary>
        /// 获取位置
        /// </summary>
        /// <returns></returns>
        public async static Task<Dictionary<string, string>> GetPosition()
        {
            try
            {
                Geolocator geo = null;
                if (geo == null)
                {
                    geo = new Geolocator();
                    geo.DesiredAccuracyInMeters = 100;
                }
                Geoposition pos = await geo.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(10));
                Dictionary<string, string> dicPos = new Dictionary<string, string>();
                dicPos.Add("lat", pos.Coordinate.Point.Position.Latitude.ToString());
                dicPos.Add("lon", pos.Coordinate.Point.Position.Longitude.ToString());
                return dicPos;
            }
            catch (Exception)
            {
                MessageDialog dialog = new MessageDialog("请开启GPS");
                return null;
            }
        }
    }
}
