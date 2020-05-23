using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlightControlWeb.Models
{
    public class FlightWrapper
    {
        private Flight flight;
        public string Id { get { return flight.Id; } }
        public double Longitude { get { return flight.Longitude; } }
        public double Latitude { get { return flight.Latitude; } }
        public int Passengers { get { return flight.Passengers; } }
        public string Company { get { return flight.Company; } }
        public DateTime DateTime { get { return flight.DateTime; } }
        public bool Is_external { get { return flight.Is_external; } }
        public List<Dictionary<string, double>> Segments { get { return flight.Segments; } }
        public DateTime EndTime { get; private set; }

        public FlightWrapper(Flight flight)
        {
            this.flight = flight;
            SetEndTime();
        }

        public void UpdateLocation(DateTime time)
        {
            int i = 0;
            DateTime tempTime = DateTime.AddSeconds(Segments[0]["timespan_seconds"]);
            foreach (Dictionary<string, double> segment in Segments)
            {
                if (DateTime.Compare(time, tempTime) > 0)
                {
                    tempTime = DateTime.AddSeconds(segment["timespan_seconds"]);
                    i++;
                }
                else
                {
                    break;
                }
            }
            //calculate how much progress the plane has made in it's current segment:
            double relativeProgress = 1 - ((tempTime - time).TotalSeconds / Segments[i]["timespan_seconds"]);
            flight.Latitude = (1 - relativeProgress) * Segments[i]["latitude"] + relativeProgress * Segments[i + 1]["latitude"];
            flight.Longitude = (1 - relativeProgress) * Segments[i]["longitude"] + relativeProgress * Segments[i + 1]["longitude"];
        }

        private void SetEndTime()
        {
            DateTime tempTime = DateTime;
            foreach (Dictionary<string, double> segment in Segments)
            {
                tempTime = tempTime.AddSeconds(segment["timespan_seconds"]);
            }
            EndTime = tempTime;
        }

        public dynamic getJson()
        {
            return JsonConvert.SerializeObject(this.flight);
        }
    }
}
