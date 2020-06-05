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
        [TestCase("2020-12-28T23:43:33Z")]
        public void FlightsModel_Sync_all(string time)
        {
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            Mock<FlightsModel> mockModel = new Mock<FlightsModel>();
            List<Flight> response = new List<Flight>();
            Server server = new Server();
            Flight flight = SetFlight();
            response.Add(flight);
            mockModel.Setup(model => model.GetCurrentFromServer(It.IsAny<Server>(), It.IsAny<string>(), It.IsAny<List<Flight>>()))
                .Callback<Server, string, List<Flight>>((server, time, response) => Task.FromResult(response))
                .ReturnsAsync((Server server, string time, List<Flight> response) => response);
            FlightsController controller = new FlightsController(cache);
            controller.SetModel(mockModel.Object);
            Task<List<Flight>> flights = controller.GetAllFlightsByTimeAsync("time");
            Assert.AreEqual(flights, response);
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
