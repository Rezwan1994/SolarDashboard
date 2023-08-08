using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solar.Models.BEL
{
    public class GenPower
    {
        public Int64 AID { get; set; }
        public string DeviceID { get; set; }
 
        public Int64 CurrentPower { get; set; }
        public Int64 TotalPower { get; set; }
        public DateTime SetDateTime { get; set; }
        public DateTime SetTimeStamp { get; set; }

    }
}
