using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        public string Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int Passengers { get; set; }
        public string Company { get; set; }
        public DateTime DateTime { get; set; }
        public bool Is_external { get; set; }
        public List<Dictionary<string, double>> Segments { get; set; }
    }
}
