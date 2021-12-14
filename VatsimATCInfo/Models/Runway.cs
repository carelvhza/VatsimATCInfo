using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VatsimATCInfo.Models
{
    public class Runway
    {
        public string ICAO { get; set; }
        public string Primary { get; set; }
        public int PrimaryDegrees { get; set; }
        public string Secondary { get; set; }
        public int SecondaryDegrees { get; set; }
        public double PrimaryLat { get; set; }
        public double PrimaryLon { get; set; }
        public double SecondaryLat { get; set; }
        public double SecondaryLon { get; set; }
    }
}