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

		public List<dynamic> GetInnerFlightsByTime(string time)
        {
			List<dynamic> currFlights = new List<dynamic>();
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

		public List<dynamic> GetAllFlightsByTime(string time)
		{
			List<dynamic> currFlights = GetInnerFlightsByTime(time);
			HttpClient client = new HttpClient();
			var servers = new List<Server>();
			if (_cache.TryGetValue("ServerList", out servers))
			{
				foreach (Server server in servers)
				{
					GetCurrentFromServer(server, currFlights);
				}
			}
			return currFlights;
		}

		public async void GetCurrentFromServer(Server server, List<dynamic> currFlights)
		{
			HttpClient client = new HttpClient();
			DateTime now = DateTime.UtcNow;
			string timeStr = now.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "T"
				+ now.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + "Z";
			var response = await client.GetStringAsync(server.ServerURL + "/api/Flights?relative_to=" + timeStr);
			dynamic json = JsonConvert.DeserializeObject(response);
			foreach (var item in json)
			{
				currFlights.Add(item);
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

		private void IterateFlights(List<dynamic> currFlights, DateTime time)
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
					currFlights.Add(flight.Value.getJson());
				}
			}
		}
	}
}