using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using VatsimATCInfo.Models;

namespace VatsimATCInfo
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DataStore.LoadData();

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
