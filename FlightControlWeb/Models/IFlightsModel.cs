using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    interface IFlightsModel
    {
        public List<dynamic> GetInnerFlightsByTime(string time);
        public List<dynamic> GetAllFlightsByTime(string time);
        public void GetCurrentFromServer(Server server, List<dynamic> currFlights);
        public bool DeleteFlight(string id);
    }
}
