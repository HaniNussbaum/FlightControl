using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace FlightControlWeb.Models
{

	public class FlightsModel
	{
		private IMemoryCache _cache;

		public FlightsModel(IMemoryCache cache)
		{
			_cache = cache;
		}

		public List<Flight> GetInnerFlightsByTime(string time)
        {
	
			List<Flight> currFlights = new List<Flight>();
			try
			{
				DateTime relativeTime = parseDateTime(time);
				IterateFlights(currFlights, relativeTime);
				return currFlights;
			} catch (Exception e)
			{
				e.ToString();
				//time in wrong format
				return null;
			}


        }

		public async Task<List<Flight>> GetAllFlightsByTimeAsync(string time)
		{
			List<Flight> currFlights = GetInnerFlightsByTime(time);
			if (currFlights == null)
			{
				currFlights = new List<Flight>();
			}
			HttpClient client = new HttpClient();
			var servers = new List<Server>();
			if (_cache.TryGetValue("ServerList", out servers))
			{
				foreach (Server server in servers)
				{
					List<Flight> list = await GetCurrentFromServer(server, time, currFlights);
					currFlights.AddRange(list);
				}
			}
			return currFlights;
		}

		public async Task<List<Flight>> GetCurrentFromServer(Server server, string time, List<Flight> currFlights)
		{
			HttpClient client = new HttpClient();
			var response = await client.GetStringAsync(server.ServerURL + "/api/Flights?relative_to=" + time);
			return JsonConvert.DeserializeObject<List<Flight>>(response);
		}

		public bool DeleteFlight(string id)
		{
			Dictionary<string, FlightWrapper> allFlights = _cache.Get<Dictionary<string, FlightWrapper>>("flights");
			return allFlights.Remove(id);
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
				System.Diagnostics.Debug.WriteLine("all flights is null");
				return;
			}
			foreach (var flight in allFlights)
			{
				System.Diagnostics.Debug.WriteLine("in loop model");
				if (DateTime.Compare(time, flight.Value.EndTime) <= 0 && DateTime.Compare(time, flight.Value.DateTime) > 0)
				{
					System.Diagnostics.Debug.WriteLine(flight.Value.Id);
					flight.Value.UpdateLocation(time);
					currFlights.Add(flight.Value.getFlight());
				}
			}
		}
	}
}