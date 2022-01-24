using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VatsimATCInfo.Models
{
    public class LoaderData
    {
        public bool Loaded { get; set; }
        public int TotalPercentage { get; set; }
        public string LoadingStatus { get; set; }
    }
}