using Boerman.OpenAip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VatsimATCInfo.Models
{
    public class AirportAirspace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AirspaceCategory Category { get; set; }
        public List<double[]> Polygon { get; set; }
    }
}