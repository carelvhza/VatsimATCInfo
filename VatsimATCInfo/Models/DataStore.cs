﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace VatsimATCInfo.Models
{
    public class DataStore
    {
        private static List<AirportData> _airports = new List<AirportData>();
        private static List<Country> _countries = new List<Country>();
        private static List<Runway> _runways = new List<Runway>();

        public static List<AirportData> GetAirports()
        {
            return _airports;
        }

        public static void LoadData()
        {
            LoadCountries();
            LoadRunways();
            LoadAirports();
        }

        private static string _getNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
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
                if (split.Length == 14)
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
    }
}