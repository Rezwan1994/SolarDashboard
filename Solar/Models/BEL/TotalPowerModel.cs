using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solar.Models.BEL
{
    public class TotalPowerModel
    {
        public double PowerGenerated { get; set; }
        public DateTime GeneratedDate { get; set; }
    }

    public class GeModel
    {
        public double PowerGenerated { get; set; }
        public DateTime GeneratedDate { get; set; }
    }
    public class GenPowerModel
    {
        public string DeviceID { get; set; }
        public Int64 CurrentPower { get; set; }
        public Int64 TotalPower { get; set; }
        public DateTime SetDate { get; set; }
        public int DifferenceTotalPower { get; set; }
    }
    public class DayWiseModel
    {
        public int YYYYMMMM { get; set; }

        public double DAY_01 { get; set; }

        public double DAY_02 { get; set; }

        public double DAY_03 { get; set; }

        public double DAY_04 { get; set; }

        public double DAY_05 { get; set; }

        public double DAY_06 { get; set; }

        public double DAY_07 { get; set; }

        public double DAY_08 { get; set; }

        public double DAY_09 { get; set; }

        public double DAY_10 { get; set; }

        public double DAY_11 { get; set; }

        public double DAY_12 { get; set; }

        public double DAY_13 { get; set; }

        public double DAY_14 { get; set; }

        public double DAY_15 { get; set; }

        public double DAY_16 { get; set; }

        public double DAY_17 { get; set; }

        public double DAY_18 { get; set; }

        public double DAY_19 { get; set; }

        public double DAY_20 { get; set; }

        public double DAY_21 { get; set; }

        public double DAY_22 { get; set; }

        public double DAY_23 { get; set; }

        public double DAY_24 { get; set; }

        public double DAY_25 { get; set; }

        public double DAY_26 { get; set; }

        public double DAY_27 { get; set; }

        public double DAY_28 { get; set; }

        public double DAY_29 { get; set; }

        public double DAY_30 { get; set; }

        public double DAY_31 { get; set; }
    }
}
