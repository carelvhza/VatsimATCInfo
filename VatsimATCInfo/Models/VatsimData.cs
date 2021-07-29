using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VatsimATCInfo.Models
{
    public class General
    {
        public int version { get; set; }
        public int reload { get; set; }
        public string update { get; set; }
        public DateTime update_timestamp { get; set; }
        public int connected_clients { get; set; }
        public int unique_users { get; set; }
    }

    public class FlightPlan
    {
        public string flight_rules { get; set; }
        public string aircraft { get; set; }
        public string aircraft_faa { get; set; }
        public string aircraft_short { get; set; }
        public string departure { get; set; }
        public string arrival { get; set; }
        public string alternate { get; set; }
        public string cruise_tas { get; set; }
        public string altitude { get; set; }
        public string deptime { get; set; }
        public string enroute_time { get; set; }
        public string fuel_time { get; set; }
        public string remarks { get; set; }
        public string route { get; set; }
        public int revision_id { get; set; }
    }

    public class Pilot
    {
        public int cid { get; set; }
        public string name { get; set; }
        public string callsign { get; set; }
        public string server { get; set; }
        public int pilot_rating { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int altitude { get; set; }
        public int groundspeed { get; set; }
        public string transponder { get; set; }
        public int heading { get; set; }
        public double qnh_i_hg { get; set; }
        public int qnh_mb { get; set; }
        public FlightPlan flight_plan { get; set; }
        public DateTime logon_time { get; set; }
        public DateTime last_updated { get; set; }

        public string dep_airport_name { get; set; }
        public string arr_airport_name { get; set; }
        public double distance_from_dep { get; set; }
        public double distance_to_arr { get; set; }
        public string status { get; set; }
        public int calculated_arrival_time { get; set; }
    }

    public class Controller
    {
        public int cid { get; set; }
        public string name { get; set; }
        public string callsign { get; set; }
        public string frequency { get; set; }
        public int facility { get; set; }
        public int rating { get; set; }
        public string server { get; set; }
        public int visual_range { get; set; }
        public List<string> text_atis { get; set; }
        public DateTime last_updated { get; set; }
        public DateTime logon_time { get; set; }
    }

    public class Ati
    {
        public int cid { get; set; }
        public string name { get; set; }
        public string callsign { get; set; }
        public string frequency { get; set; }
        public int facility { get; set; }
        public int rating { get; set; }
        public string server { get; set; }
        public int visual_range { get; set; }
        public string atis_code { get; set; }
        public List<string> text_atis { get; set; }
        public DateTime last_updated { get; set; }
        public DateTime logon_time { get; set; }
    }

    public class Server
    {
        public string ident { get; set; }
        public string hostname_or_ip { get; set; }
        public string location { get; set; }
        public string name { get; set; }
        public int clients_connection_allowed { get; set; }
    }

    public class Prefile
    {
        public int cid { get; set; }
        public string name { get; set; }
        public string callsign { get; set; }
        public FlightPlan flight_plan { get; set; }
        public DateTime last_updated { get; set; }
    }

    public class Facility
    {
        public int id { get; set; }
        public string @short { get; set; }
        public string @long { get; set; }
    }

    public class Rating
    {
        public int id { get; set; }
        public string @short { get; set; }
        public string @long { get; set; }
    }

    public class PilotRating
    {
        public int id { get; set; }
        public string short_name { get; set; }
        public string long_name { get; set; }
    }

    public class VatsimData
    {
        public General general { get; set; }
        public List<Pilot> pilots { get; set; }
        public List<Controller> controllers { get; set; }
        public List<Ati> atis { get; set; }
        public List<Server> servers { get; set; }
        public List<Prefile> prefiles { get; set; }
        public List<Facility> facilities { get; set; }
        public List<Rating> ratings { get; set; }
        public List<PilotRating> pilot_ratings { get; set; }
    }
}