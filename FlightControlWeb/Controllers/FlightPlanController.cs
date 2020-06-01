using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using FlightControlWeb.Models;
using Newtonsoft.Json;
using System.Net;

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
            try
            {
                FlightWrapper newFlight = new FlightWrapper(flightPlan);
                return this.addFlight(newFlight);
            }
            catch (Exception e)
            {
                Dictionary<string, string> error = new Dictionary<string, string>();
                error["Status"] = "error";
                error["Message"] = e.Message;
                Response.StatusCode = 415;
                return JsonConvert.SerializeObject(error);
            }
        }

        private ActionResult<dynamic> addFlight(FlightWrapper flight)
        {
            var flights = new Dictionary<string, FlightWrapper>();
            if (!_cache.TryGetValue("flights", out flights))
            {
                if (flights == null)
                {
                    flights = new Dictionary<string, FlightWrapper>();
                }
                _cache.Set("flights", flights);
            }
            flights.Add(flight.Id, flight);
            return Created(flight.Id, flight);
        }
    }
}