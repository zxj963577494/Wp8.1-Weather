using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Weather.Utils
{
    public static class ApplicationHelper
    {
        public static void Exit()
        {
            Application.Current.Exit();
        }

    }
}
