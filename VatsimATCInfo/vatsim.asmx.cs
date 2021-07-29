using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using VatsimATCInfo.Models;

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
            return val;
        }
    }
}
