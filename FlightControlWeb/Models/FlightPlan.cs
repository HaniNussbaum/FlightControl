using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        [JsonProperty("passengers")]
        public int Passengers { get; set; }
        [JsonProperty("company_name")]
        public string Company { get; set; }
        [JsonProperty("initial_location")]
        public Dictionary<string, dynamic> InitialLocation { get; set; }
        [JsonProperty("segments")]
        public List<Dictionary<string, dynamic>> Segments { get; set; }
       
    }
}
