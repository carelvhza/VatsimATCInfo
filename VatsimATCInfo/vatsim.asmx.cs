using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using VatsimATCInfo.Models;
using csharp_metar_decoder;
using csharp_metar_decoder.entity;
using System.IO;
using System.Device;
using System.Device.Location;
using System.Web.Hosting;

namespace VatsimATCInfo
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class vatsim : WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public VatsimData GetData()
        {
            var client = new RestClient("https://data.vatsim.net/");
            var request = new RestRequest("v3/vatsim-data.json", DataFormat.Json);
            var response = client.Get(request);
            var val = JsonConvert.DeserializeObject<VatsimData>(response.Content);

            var mainFile = File.ReadAllLines(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "airports.dat"));
            var airportData = new List<AirportData>();
            var c = 0;
            foreach (var item in mainFile)
            {
                c++;
                var split = item.Split(',');
                if (split.Length == 14)
                {
                    airportData.Add(new AirportData()
                    {
                        Id = int.Parse(split[0]),
                        Name = split[1],
                        ShortName = split[2],
                        Country = split[3],
                        IATA = split[4],
                        ICAO = split[5],
                        Latitude = double.Parse(split[6]),
                        Longitude = double.Parse(split[7]),
                        Altitude = int.Parse(split[8])
                    });
                }
            }

            foreach (var pilot in val.pilots.Where(a => a.flight_plan != null))
            {
                var depAirport = airportData.FirstOrDefault(ap => ap.ICAO == pilot.flight_plan.departure);
                var arrAirport = airportData.FirstOrDefault(ap => ap.ICAO == pilot.flight_plan.arrival);

                if (depAirport != null && arrAirport != null)
                {
                    var planeCoord = new GeoCoordinate(pilot.latitude, pilot.longitude);

                    var airportCoord = new GeoCoordinate(depAirport.Latitude, depAirport.Longitude);
                    var distance = planeCoord.GetDistanceTo(airportCoord);
                    pilot.distance_from_dep = distance;
                    pilot.dep_airport_name = depAirport.Name;

                    airportCoord = new GeoCoordinate(arrAirport.Latitude, arrAirport.Longitude);
                    distance = planeCoord.GetDistanceTo(airportCoord);
                    pilot.distance_to_arr = distance;
                    pilot.arr_airport_name = arrAirport.Name;

                    if (pilot.distance_from_dep > 6000 && pilot.distance_to_arr > 6000 && pilot.groundspeed > 40)
                    {
                        pilot.status = "Airborne";
                    }
                    else if (pilot.distance_from_dep <= 6000 && pilot.altitude <= depAirport.Altitude + 10 && pilot.groundspeed > 0 && pilot.groundspeed < 40)
                    {
                        pilot.status = "Taxi Out";
                    }
                    else if (pilot.distance_from_dep <= 6000 && pilot.altitude > depAirport.Altitude + 10 && pilot.groundspeed > 40)
                    {
                        pilot.status = "Departing";
                    }
                    else if (pilot.distance_to_arr <= 6000 && pilot.groundspeed > 40 && pilot.altitude > arrAirport.Altitude + 10)
                    {
                        pilot.status = "Arriving";
                    }
                    else if (pilot.distance_to_arr <= 6000 && pilot.altitude <= arrAirport.Altitude + 10 && pilot.groundspeed < 40 && pilot.groundspeed != 0)
                    {
                        pilot.status = "Taxi In";
                    }
                    else if (pilot.distance_to_arr <= 6000 && pilot.altitude <= arrAirport.Altitude + 10 && pilot.groundspeed == 0)
                    {
                        pilot.status = "Arrived";
                    }
                    else
                    {
                        pilot.status = "Preparing";
                    }

                    if (pilot.groundspeed > 0)
                    {
                        var kph = pilot.groundspeed * 1.852d;
                        var mpm = kph * 16.666d;
                        var minutes = pilot.distance_to_arr / mpm;

                        if (minutes > 0)
                        {
                            var targetTime = DateTime.UtcNow.AddMinutes(minutes);
                            pilot.calculated_arrival_time = int.Parse(targetTime.ToString("HHmm"));
                        }
                        else pilot.calculated_arrival_time = 0000;
                    }
                    else
                    {
                        pilot.calculated_arrival_time = 0000;
                    }
                }
                else
                {
                    pilot.status = "Invalid aiport ICAO";
                }
            }


            return val;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DecodedMetar GetMetar(string icao)
        {
            var client = new RestClient("https://metar.vatsim.net/");
            var request = new RestRequest($"metar.php?id={icao}", DataFormat.Json);
            var response = client.Get(request);
            var metar = MetarDecoder.ParseWithMode(response.Content);
            return metar;
        }
    }
}
