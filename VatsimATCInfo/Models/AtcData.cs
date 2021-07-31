using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VatsimATCInfo.Models
{   // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Transceiver
    {
        public int id { get; set; }
        public int frequency { get; set; }
        public double latDeg { get; set; }
        public double lonDeg { get; set; }
        public double heightMslM { get; set; }
        public double heightAglM { get; set; }
    }

    public class RadioSource
    {
        public string callsign { get; set; }
        public List<Transceiver> transceivers { get; set; }
    }

    public class AtcData
    {
        public List<RadioSource> Sources { get; set; }
    }



}