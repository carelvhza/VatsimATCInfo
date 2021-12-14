using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VatsimATCInfo.Models
{
    public class AirportData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Country { get; set; }
        public string IATA { get; set; }
        public string ICAO { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }        
        public int Altitude { get; set; }
        public List<Runway> Runways { get; set; }
        public string CountryCode { get; set; }
        public string Municipality { get; set; }
        public string Continent { get; set; }
    }
}