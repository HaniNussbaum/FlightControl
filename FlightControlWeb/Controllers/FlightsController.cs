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
    [Route("api/[controller]")]
    [ApiController]

    public class FlightsController : ControllerBase
    {
        private readonly FlightsModel _model;

        public FlightsController(FlightsModel model) {
            _model = model;
        }

        // GET: /api/Flights?relative_to=<DATE_TIME>
        [HttpGet, Route("Flights?relative_to={time}")]
        public IEnumerable<dynamic> GetInnerFlights(string time){
            return _model.GetInnerFlightsByTime(time);
        }

        // GET: /api/Flights?relative_to=<DATE_TIME>&sync_all
        [HttpGet, Route("Flights?relative_to={time}&sync_all")]
        public IEnumerable<dynamic> GetAllFlights(string time)
        {
            return _model.GetAllFlightsByTime(time);
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