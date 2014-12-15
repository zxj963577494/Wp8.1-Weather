using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class GetSettingSwitchesRespose
    {
        public List<Model.Switchable> Switches { get; set; }
    }
}
