using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using FlightControlWeb.Models;
using Newtonsoft.Json;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public FlightPlanController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet("{id}")]
        public ActionResult<dynamic> GetFlightPlan(string id)
        {
            var flights = new Dictionary<string, FlightWrapper>();
            if (!_cache.TryGetValue("flights", out flights))
            {
                return NotFound(id);             
            }
            FlightWrapper flight = flights[id];
            if (flight == null)
            {
                return NotFound(id);
            }

            return Ok(flight.getFlightPlanJson());
        }

        [HttpPost]
        public ActionResult<dynamic> AddFlightPlan([FromBody] FlightPlan flightPlan)
        {
            FlightWrapper newFlight = new FlightWrapper(flightPlan);
            var flights = new Dictionary<string, FlightWrapper>();
            if (!_cache.TryGetValue("flights", out flights))
            {
                if (flights == null)
                {
                    flights = new Dictionary<string, FlightWrapper>();
                    //System.Diagnostics.Debug.WriteLine("serversnull");
                }
                _cache.Set("flights", flights);
            }
            flights.Add(newFlight.Id, newFlight);
            return Created(newFlight.Id, newFlight);
        }

    }
}