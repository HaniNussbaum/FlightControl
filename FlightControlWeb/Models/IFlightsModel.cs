using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightsModel
    {
        public Task<List<Flight>> GetCurrentFromServer(Server server, string time, List<Flight> currFlights);
    }
}
