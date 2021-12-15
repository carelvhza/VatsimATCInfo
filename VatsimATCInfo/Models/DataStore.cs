using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Boerman.OpenAip;
using Newtonsoft.Json;

namespace VatsimATCInfo.Models
{
    public class DataStore
    {
        private static List<AirportData> _airports = new List<AirportData>();
        private static List<Country> _countries = new List<Country>();
        private static List<Runway> _runways = new List<Runway>();
        private static List<Airspace> _airspaces = new List<Airspace>();
        private static List<Airline> _airlines = new List<Airline>();

        public static List<AirportData> GetAirports()
        {
            return _airports;
        }

        public static List<Airspace> GetAirspaces()
        {
            return _airspaces;
        }

        public static void LoadData()
        {           
            LoadCountries();
            LoadAirlines();
            LoadRunways();
            LoadAirports();
        }

        private static string _getNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public static Airline GetAirline(string icao)
        {
            return _airlines.FirstOrDefault(al => al.icao == icao);
        }

        private static void LoadAirports()
        {
            var mainFile = File.ReadAllLines(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "airports_new.dat"));
            var airportData = new List<AirportData>();
            var c = 0;
            foreach (var item in mainFile)
            {
                c++;
                var split = item.Split(',');
                if (split.Length == 10)
                {
                    var runwaysForThisIcao = _runways.Where(rw => rw.ICAO == split[4]);
                    var country = _countries.FirstOrDefault(ct => ct.Code == split[2])?.Name;
                    if (string.IsNullOrEmpty(country))
                    {
                        country = "Unknown";
                    }
                    var counter = 0;
                    airportData.Add(new AirportData()
                    {
                        Id = NextNumber(ref counter),
                        Name = split[0],
                        ShortName = split[1],
                        CountryCode = split[2],
                        IATA = split[3],
                        ICAO = split[4],
                        Latitude = double.Parse(split[5]),
                        Longitude = double.Parse(split[6]),
                        Altitude = (!string.IsNullOrEmpty(split[7]) ? int.Parse(split[7]) : 0),
                        Runways = runwaysForThisIcao.ToList(),
                        Continent = split[8],
                        Municipality = split[9],
                        Country = country
                    });
                }
            }
            _airports = airportData;
        }

        private static int NextNumber(ref int counter)
        {
            counter++;
            return counter;
        }

        private static void LoadCountries()
        {
            var mainFile = File.ReadAllLines(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "countries.dat"));
            var countryData = new List<Country>();

            var c = 0;
            foreach (var item in mainFile)
            {
                c++;
                var split = item.Split(',');
                if (split.Length == 3)
                {
                    countryData.Add(new Country()
                    {
                        Code = split[0],
                        Name = split[1],
                        Continent = split[2]
                    });
                }
            }
            _countries = countryData;
        }

        private static void LoadAirlines()
        {
            var mainFile = File.ReadAllText(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "airline_data_daisycon.json"));
            var airlineData = JsonConvert.DeserializeObject<List<Airline>>(mainFile);
            _airlines = airlineData;          
        }

        public static Airspace GetAirspace(string countryCode, string name, string suffix)
        {
            var path = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, $"openaipdata",$"{countryCode}_asp.aip");
           
            if (File.Exists(path))
            {
                var data = File.ReadAllText(path);
                if (!string.IsNullOrEmpty(data))
                {
                    var countryAirspaceData = Boerman.OpenAip.Parsers.Airspace.Parse(data).ToList();
                    if (countryAirspaceData.Any())
                    {
                        var result = countryAirspaceData.FirstOrDefault(a =>
                            a.Name.ToUpper().Contains(name.ToUpper())
                            && 
                                (
                                    a.Name.ToUpper().Contains(" CTLZ")
                                    || a.Name.ToUpper().Contains(" CLTZ")
                                    || a.Name.ToUpper().Contains(" CLASS B")
                                    || a.Name.ToUpper().Contains(" CTR")
                                    || a.Name.ToUpper().Contains(" CONTROL ZONE")
                                )
                            );
                       
                        return result;
                    }
                    else
                        return null;
                }
            }
            else
            {
                throw new Exception("Path: " + path);
            }
            return null;
        }

        private static void LoadRunways()
        {
            var mainFile = File.ReadAllLines(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "runways.csv"));
            var runwayData = new List<Runway>();
            var c = 0;
            foreach (var item in mainFile)
            {
                c++;
                var split = item.Split(',');
                if (split.Length == 20)
                {
                    var primaryWithoutLetters = _getNumbers(split[8]?.Replace("\"", ""));
                    var secondaryWithoutLetters = _getNumbers(split[14]?.Replace("\"", ""));
                    int primaryDegrees = 0;
                    int secondaryDegrees = 0;

                    int.TryParse(primaryWithoutLetters, out primaryDegrees);
                    int.TryParse(secondaryWithoutLetters, out secondaryDegrees);

                    var rw = new Runway();
                    rw.ICAO = split[2]?.Replace("\"", "");
                    rw.Primary = split[8]?.Replace("\"", "");
                    rw.PrimaryDegrees = primaryDegrees;
                    rw.Secondary = split[14]?.Replace("\"", "");
                    rw.SecondaryDegrees = secondaryDegrees;
                    rw.PrimaryLat = !string.IsNullOrEmpty(split[9]) ? Convert.ToDouble(split[9]) : 0.0;
                    rw.PrimaryLon = !string.IsNullOrEmpty(split[10]) ? Convert.ToDouble(split[10]) : 0.0;
                    rw.SecondaryLat = !string.IsNullOrEmpty(split[15]) ? Convert.ToDouble(split[15]) : 0.0;
                    rw.SecondaryLon = !string.IsNullOrEmpty(split[16]) ? Convert.ToDouble(split[16]) : 0.0;
                    runwayData.Add(rw);
                }
            }
            _runways = runwayData;
        }
    }
}