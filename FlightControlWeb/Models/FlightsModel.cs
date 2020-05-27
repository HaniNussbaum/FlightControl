using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
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

		public List<FlightWrapper> GetInnerFlightsByTime(string time)
        {
	
			List<FlightWrapper> currFlights = new List<FlightWrapper>();
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

		public List<FlightWrapper> GetAllFlightsByTime(string time)
		{
			List<FlightWrapper> currFlights = GetInnerFlightsByTime(time);
			HttpClient client = new HttpClient();
			var servers = new List<Server>();
			if (_cache.TryGetValue("ServerList", out servers))
			{
				foreach (Server server in servers)
				{
					GetCurrentFromServer(server, currFlights, time);
				}
			}
			return currFlights;
		}

		public async void GetCurrentFromServer(Server server, List<FlightWrapper> currFlights, string time)
		{
			HttpClient client = new HttpClient();
			var response = await client.GetStringAsync(server.ServerURL + "/api/Flights?relative_to=" + time);
			List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(response);
			foreach (Flight flight in flights)
			{
				FlightWrapper temp = new FlightWrapper(flight);
				currFlights.Add(temp);
			}
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

		private void IterateFlights(List<FlightWrapper> currFlights, DateTime time)
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
					currFlights.Add(flight.Value);
				}
			}
		}
	}
}