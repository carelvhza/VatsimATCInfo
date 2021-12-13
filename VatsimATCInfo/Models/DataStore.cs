using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace VatsimATCInfo.Models
{
    public class DataStore
    {
        public static List<Runway> _runways = new List<Runway>();
        public static List<AirportData> _airports = new List<AirportData>();


        public static List<AirportData> GetAirports()
        {
            return _airports;
        }

        public static void LoadData()
        {
            LoadRunways();
            LoadAirports();
        }

        private static void LoadRunways()
        {
            var mainFile = File.ReadAllLines(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "runways.dat"));
            var runwayData = new List<Runway>();
            var c = 0;
            foreach (var item in mainFile)
            {
                c++;
                var split = item.Split(',');
                if (split.Length == 5)
                {
                    var primaryWithoutLetters = _getNumbers(split[1]);
                    var secondaryWithoutLetters = _getNumbers(split[3]);
                    int primaryDegrees = 0;
                    int secondaryDegrees = 0;

                    int.TryParse(primaryWithoutLetters, out primaryDegrees);
                    int.TryParse(secondaryWithoutLetters, out secondaryDegrees);


                    runwayData.Add(new Runway()
                    {
                        ICAO = split[0],
                        Primary = split[1],
                        PrimaryDegrees = primaryDegrees,
                        Secondary = split[3],
                        SecondaryDegrees = secondaryDegrees
                    });
                }
            }
            _runways = runwayData;
        }

        private static void LoadAirports()
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
                    var runwaysForThisIcao = _runways.Where(rw => rw.ICAO == split[5]);

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
                        Altitude = int.Parse(split[8]),
                        Runways = runwaysForThisIcao.ToList()
                    });
                }
            }         
            _airports = airportData;
        }

        private static string _getNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }
    }
}