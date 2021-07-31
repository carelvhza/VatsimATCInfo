using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static VatsimATCInfo.Helpers.MainEnums;

namespace VatsimATCInfo.Helpers
{
    public class Communication
    {
        private static string _dataUrl = "https://data.vatsim.net";
        private static string _metarUrl = "https://metar.vatsim.net";
        private static string _vatsimDataRequest = "v3/vatsim-data.json";
        private static string _vatsimTransceiverRequest = "v3/transceivers-data.json";
        private static string _metarRequest = "metar.php?id=";
        internal static T DoCall<T>(DataCalls call, string icao = "")
        {            
            RestClient client = null;
            RestRequest request = null;            
            switch (call)
            {
                case DataCalls.VatsimData:
                    client = new RestClient(_dataUrl);
                    request = new RestRequest(_vatsimDataRequest, DataFormat.Json);
                    break;
                case DataCalls.TransceiverData:
                    client = new RestClient(_dataUrl);
                    request = new RestRequest(_vatsimTransceiverRequest, DataFormat.Json);
                    break;
                case DataCalls.MetarData:
                    client = new RestClient(_metarUrl);
                    request = new RestRequest($"{_metarRequest}{icao}", DataFormat.Json);
                    break;
            }
            var response = client.Get(request);
            return JsonConvert.DeserializeObject<T>(response.Content);

        }
    }
}