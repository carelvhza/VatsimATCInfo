using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VatsimATCInfo.Models
{
    public class Airline
    {
        public int id { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public string callsign { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public string iata { get; set; }
        public string icao { get; set; }
        public string logo { get; set; }
    }
}