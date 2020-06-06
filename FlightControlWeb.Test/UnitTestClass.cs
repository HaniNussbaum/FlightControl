using NUnit.Framework;
using System;
using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace FlightControlWeb.Test
{
    [TestFixture]
    public class UnitTestClass
    {
        private static FlightPlan plan;
        
        [TestCase("latitude range is -90 - +90")]
        [TestCase("longitude range is -180 - +180")]
        [TestCase("number of passengers can not be negative")]
        [TestCase("company name missing")]
        [TestCase("segment timespan can not be negative")]
        public void FlightWrapper_wrong_format(string message)
        {
            initializeFlightPlan();
            if (message == "date_time format is incorrect")
            {
                plan.InitialLocation["date_time"] = "2020-12-28T23-40:33Z";
            } else if (message == "latitude range is -90 - +90")
            {
                plan.InitialLocation["latitude"] = 100;
            } else if (message == "longitude range is -180 - +180")
            {
                plan.InitialLocation["longitude"] = -200;
            } else if (message == "number of passengers can not be negative")
            {
                plan.Passengers = -1;
            } else if (message == "company name missing")
            {
                plan.Company = "";
            } else if (message == "segment timespan can not be negative")
            {
                plan.Segments[0]["timespan_seconds"] = -2;
            }
            try
            {
                FlightWrapper shouldThrow = new FlightWrapper(plan);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(message, e.Message);
            }
        }

        [Test]
        public void FlightsModel_Sync_all()
        {
            //set up:
            string time = DateTime.UtcNow.ToString();
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            Mock<IFlightsModel> mockModel = new Mock<IFlightsModel>();
            List<Flight> response = new List<Flight>();
            Server server = new Server();
            server.ServerId = 123;
            server.ServerURL = "www.test.com";
            List<Server> servers = new List<Server>();
            servers.Add(server);
            cache.Set("ServerList", servers);
            Flight flight = SetFlight();
            response.Add(flight);
            List<Flight> curr = new List<Flight>();
            //mock method definition:
            mockModel.Setup(model => model.GetCurrentFromServer(It.IsAny<Server>(), It.IsAny<string>(), It.IsAny<List<Flight>>()))
                .Returns((Server server, string time, List<Flight> curr) => { return Task.FromResult(response); });
            FlightsController controller = new FlightsController(cache);
            controller.SetModel(mockModel.Object);
            //execution:
            Task<List<Flight>> flights = controller.GetAllFlightsByTimeAsync(time);
            //assert:
            Assert.AreEqual(flights.Result, response);
        }

        private Flight SetFlight()
        {
            Flight flight = new Flight();
            flight.Company = "swiss";
            flight.Id = "AL280528";
            flight.Is_external = true;
            flight.Latitude = 33.3;
            flight.Longitude = 32.8;
            flight.Passengers = 300;
            flight.DateTime = DateTime.UtcNow;
            return flight;
        }

        private void initializeFlightPlan()
        {
            plan = new FlightPlan();
            plan.Company = "swiss";
            plan.Passengers = 300;
            plan.InitialLocation = new Dictionary<string, dynamic>();
            plan.InitialLocation["latitude"] = 30.6;
            plan.InitialLocation["longitude"] = 30.0;
            plan.InitialLocation["date_time"] = DateTime.UtcNow;
            Dictionary<string, dynamic> segment = new Dictionary<string, dynamic>();
            segment["latitude"] = 35.4;
            segment["longitude"] = 34.2;
            segment["timespan_seconds"] = 100;
            plan.Segments = new List<Dictionary<string, dynamic>>();
            plan.Segments.Add(segment);
        }
    }
}
