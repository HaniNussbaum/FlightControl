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

	public class FlightsModel : IFlightsModel
	{
		public async Task<List<Flight>> GetCurrentFromServer(Server server, string time, List<Flight> currFlights)
		{
			HttpClient client = new HttpClient();
			var response = await client.GetAsync(server.ServerURL + "/api/Flights?relative_to=" + time);
			if (response.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<List<Flight>>(response.Content.ReadAsStringAsync().Result);
			}
			return new List<Flight>();
		}
	}
}