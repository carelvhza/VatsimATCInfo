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
using VatsimATCInfo.Helpers;
using static VatsimATCInfo.Helpers.MainEnums;

namespace VatsimATCInfo
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class vatsim : WebService
    {

        /// <summary>
        /// Returns base data used in front end
        /// </summary>
        /// <param name="icao">4 letter ICAO code</param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public VatsimData GetData(string icao)
        {
            icao = icao.ToUpper();     
            var vatsimData = Communication.DoCall<VatsimData>(DataCalls.VatsimData, icao);
            var airportData = _getAirports();            
            var currentAirport = airportData.FirstOrDefault(air => air.ICAO == icao);
            if (currentAirport == null)
            {
                return null;
            }


            vatsimData.current_airport_name = currentAirport.Name;            
            vatsimData.airport_height = currentAirport.Altitude;
            vatsimData.pilots = vatsimData.pilots.Where(pi => pi.flight_plan != null && (pi.flight_plan?.arrival == icao || pi.flight_plan?.departure == icao)).OrderBy(pi2 => pi2.callsign).ToList();

            foreach (var pilot in vatsimData.pilots.Where(a => a.flight_plan != null))
            {
                var depAirport = airportData.FirstOrDefault(arp => arp.ICAO == pilot.flight_plan.departure);
                var arrAirport = airportData.FirstOrDefault(arp => arp.ICAO == pilot.flight_plan.arrival);

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
                    var flightType = (pilot.flight_plan.arrival == icao) ? 2 : (pilot.flight_plan.departure == icao) ? 1 : 0;

                    if (pilot.distance_from_dep > 6000 && pilot.distance_to_arr > 6000 && pilot.groundspeed > 40)
                    {
                        pilot.status = "Airborne";
                        pilot.sort_order = 3;
                    }
                    else if (pilot.distance_from_dep <= 6000 && pilot.altitude <= depAirport.Altitude + 10 && pilot.groundspeed > 0 && pilot.groundspeed < 40)
                    {
                        pilot.status = "Taxi Out";
                        pilot.sort_order = (flightType == 1) ? 1 : 5;
                    }
                    else if (pilot.distance_from_dep <= 6000 && pilot.altitude > depAirport.Altitude + 10 && pilot.groundspeed > 40)
                    {
                        pilot.status = "Departing";
                        pilot.sort_order = (flightType == 1) ? 2 : 4;
                    }
                    else if (pilot.distance_to_arr <= 55500 && pilot.groundspeed > 40 && pilot.altitude > arrAirport.Altitude + 10)
                    {
                        pilot.status = "Arriving";
                        pilot.sort_order = (flightType == 1) ? 4 : 2;
                    }
                    else if (pilot.distance_to_arr <= 6000 && pilot.altitude <= arrAirport.Altitude + 10 && pilot.groundspeed < 40 && pilot.groundspeed != 0)
                    {
                        pilot.status = "Taxi In";
                        pilot.sort_order = (flightType == 1) ? 5 : 1;
                    }
                    else if (pilot.distance_to_arr <= 6000 && pilot.altitude <= arrAirport.Altitude + 10 && pilot.groundspeed == 0)
                    {
                        pilot.status = "Arrived";
                        pilot.sort_order = (flightType == 1) ? 6 : 0;
                    }
                    else
                    {
                        pilot.status = "Preparing";
                        pilot.sort_order = (flightType == 1) ? 0 : 6;
                    }

                    if (pilot.distance_to_arr <= 55500 && pilot.groundspeed > 40 && pilot.altitude > arrAirport.Altitude + 10)
                    {
                        pilot.status = "Arriving";
                        pilot.sort_order = (flightType == 1) ? 4 : 2;
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

            
            vatsimData.departures = vatsimData.pilots.Where(pi => pi.flight_plan != null && pi.flight_plan.departure == icao).ToList();
            vatsimData.departures = vatsimData.departures.OrderBy(pi => pi.sort_order).ThenBy(pi2 => pi2.flight_plan.deptime).ThenBy(pi3 => pi3.calculated_arrival_time).ToList();
            
            vatsimData.arrivals = vatsimData.pilots.Where(pi => pi.flight_plan != null && pi.flight_plan.arrival == icao).ToList();
            vatsimData.arrivals = vatsimData.arrivals.OrderBy(pi => pi.sort_order).ThenBy(pi2 => pi2.calculated_arrival_time).ThenBy(pi3 => pi3.flight_plan.deptime).ToList();


            var ap = airportData.FirstOrDefault(air => air.ICAO == icao);
            var transceivers = _getTransceivers();
            if (ap != null)
            {
                var airportCoords = new GeoCoordinate(ap.Latitude, ap.Longitude);

                foreach (var controller in vatsimData.controllers)
                {
                    var tr = transceivers.FirstOrDefault(tra => tra.callsign == controller.callsign);
                    if (tr != null && tr?.transceivers != null && tr.transceivers.Any()) 
                    {
                        var atcCoords = new GeoCoordinate(tr.transceivers[0].latDeg, tr.transceivers[0].lonDeg);
                        controller.distance_from_airport = airportCoords.GetDistanceTo(atcCoords);
                    }
                }

                vatsimData.controllers = vatsimData.controllers.Where(a => a.distance_from_airport < 200000 && a.distance_from_airport != 0).OrderBy(a2 => a2.callsign).ToList();
            }
            return vatsimData;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DecodedMetar GetMetar(string icao)
        {
            var client = new RestClient("https://metar.vatsim.net/");
            var request = new RestRequest($"metar.php?id={icao}", DataFormat.Json);
            var response = client.Get(request);
            if (response.Content == "")
            {
                return null;
            }
            var metar = MetarDecoder.ParseWithMode(response.Content);
            return metar;
        }     
        
        private List<RadioSource> _getTransceivers()
        {
            var client = new RestClient("https://data.vatsim.net/");
            var request = new RestRequest("v3/transceivers-data.json", DataFormat.Json);
            var response = client.Get(request);

            var transceivers = JsonConvert.DeserializeObject<List<RadioSource>>(response.Content);

            return transceivers;
        }

        private List<AirportData> _getAirports()
        {
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
            return airportData;
        }
    }
}
