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
        public List<dynamic> GetInnerFlights()
        {
            string time = Request.Query["relative_to"];
            if (Request.Query.ContainsKey("sync_all"))
            {
                return _model.GetAllFlightsByTime(time);
            }
            else
            {
                return _model.GetInnerFlightsByTime(time);
            }
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