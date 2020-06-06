using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        [JsonProperty("flight_id")]
        public string Id { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("passengers")]
        public int Passengers { get; set; }
        [JsonProperty("company_name")]
        public string Company { get; set; }
        [JsonProperty("date_time")]
        public DateTime DateTime { get; set; }
        [JsonProperty("is_external")]
        public bool Is_external { get; set; }
    }
}
