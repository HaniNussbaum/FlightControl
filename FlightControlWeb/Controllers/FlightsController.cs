using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using System;

namespace FlightControlWeb.Controllers
{
    [Route("api/Flights")]
    [ApiController]

    public class FlightsController : ControllerBase
    {
        private IFlightsModel _model;
        private IMemoryCache _cache;

        public FlightsController(IMemoryCache cache) {
            _model = new FlightsModel();
            _cache = cache;
        }

        public void SetModel(IFlightsModel model)
        {
            _model = model;
        }

        // GET: /api/Flights?relative_to=<DATE_TIME>
        // GET: /api/Flights?relative_to=<DATE_TIME>&sync_all
        [HttpGet]
        public ActionResult<List<Flight>> GetInnerFlights()
        {
            string time = Request.Query["relative_to"];
            List<Flight> flights;
            if (Request.Query.ContainsKey("sync_all"))
            {
                flights = GetAllFlightsByTimeAsync(time).Result;
            }
            else
            {
                flights = GetInnerFlightsByTime(time);
            }
            return Ok(flights);
        }

        ///DELETE: api/Flights/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteFlight(string id)
        {
            Dictionary<string, FlightWrapper> allFlights = _cache.Get<Dictionary<string, FlightWrapper>>("flights");
            if (allFlights.Remove(id))
            {
                return Ok(id);
            }
            return NotFound(id);
        }

        private List<Flight> GetInnerFlightsByTime(string time)
        {

            List<Flight> currFlights = new List<Flight>();
            try
            {
                DateTime relativeTime = parseDateTime(time);
                IterateFlights(currFlights, relativeTime);
                return currFlights;
            }
            catch (Exception e)
            {
                e.ToString();
                //time in wrong format
                return currFlights;
            }
        }

        public async Task<List<Flight>> GetAllFlightsByTimeAsync(string time)
        {
            List<Flight> currFlights = GetInnerFlightsByTime(time);
            if (currFlights == null)
            {
                currFlights = new List<Flight>();
            }
            var servers = new List<Server>();
            if (_cache.TryGetValue("ServerList", out servers))
            {
                foreach (Server server in servers)
                {
                    List<Flight> list = await _model.GetCurrentFromServer(server, time, currFlights);
                    currFlights.AddRange(list);
                }
            }
            return currFlights;
        }

        private DateTime parseDateTime(string time)
        {
            char[] delimeters = { ':', 'T', 'Z', '-' };
            string[] words = time.Split(delimeters);
            return new DateTime(Int32.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2]),
                Int32.Parse(words[3]), Int32.Parse(words[4]), Int32.Parse(words[5]));
        }

        private void IterateFlights(List<Flight> currFlights, DateTime time)
        {
            Dictionary<string, FlightWrapper> allFlights = _cache.Get<Dictionary<string, FlightWrapper>>("flights");
            if (allFlights == null)
            {
                return;
            }
            foreach (var flight in allFlights)
            {
                if (DateTime.Compare(time, flight.Value.EndTime) <= 0 && DateTime.Compare(time, flight.Value.DateTime) > 0)
                {
                    flight.Value.UpdateLocation(time);
                    currFlights.Add(flight.Value.getFlight());
                }
            }
        }
    }
}