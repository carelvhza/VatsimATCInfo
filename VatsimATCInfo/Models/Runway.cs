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
    }
}