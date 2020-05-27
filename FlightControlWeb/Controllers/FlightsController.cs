using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FlightControlWeb.Controllers
{
    [Route("api/Flights")]
    [ApiController]

    public class FlightsController : ControllerBase
    {
        private readonly FlightsModel _model;

        public FlightsController(IMemoryCache cache) {
            _model = new FlightsModel(cache);
        }

        // GET: /api/Flights?relative_to=<DATE_TIME>
        // GET: /api/Flights?relative_to=<DATE_TIME>&sync_all
        [HttpGet]
        public ActionResult<List<dynamic>> GetInnerFlights()
        {
            string time = Request.Query["relative_to"];
            List<FlightWrapper> flights;
            if (Request.Query.ContainsKey("sync_all"))
            {
                flights = _model.GetAllFlightsByTime(time);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("in get inner flights");
                flights = _model.GetInnerFlightsByTime(time);
            }
            List<Flight> flightsJsons = new List<Flight>();
            foreach (FlightWrapper flight in flights)
            {
                flightsJsons.Add(flight.getFlight());
                System.Diagnostics.Debug.WriteLine(flight.Id);
            }
            return Ok(flightsJsons);
        }

        ///DELETE: api/Flights/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteFlight(string id)
        {
            if (_model.DeleteFlight(id))
            {
                return Ok(id);
            }
            return NotFound(id);
        }
    }
}