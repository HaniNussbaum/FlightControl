using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlightControlWeb.Models
{
    public class FlightWrapper
    {
        [JsonIgnore]
        private Flight flight;
        [JsonIgnore]
        public string Id { get { return flight.Id; } }
        [JsonIgnore]
        public double Longitude { get { return flight.Longitude; } }
        [JsonIgnore]
        public double Latitude { get { return flight.Latitude; } }
        [JsonProperty("passengers")]
        public int Passengers { get { return flight.Passengers; } }
        [JsonProperty("company_name")]
        public string Company { get { return flight.Company; } }
        [JsonIgnore]
        public DateTime DateTime { get { return flight.DateTime; } }
        [JsonIgnore]
        public bool Is_external { get { return flight.Is_external; } }
        [JsonProperty("initial_location")]
        public Dictionary<string, dynamic> InitialLocation { get; set; }
        [JsonProperty("segments")]
        public List<Dictionary<string, dynamic>> Segments { get; set; }
        [JsonIgnore]
        public DateTime EndTime { get; private set; }

        public FlightWrapper(FlightPlan plan)
        {
            this.flight = new Flight();
            if (plan.Passengers < 0)
            {
                throw new Exception("number of passengers can not be negative");
            }
            this.flight.Passengers = plan.Passengers;
            if (plan.Company == "" || plan.Company == null)
            {
                throw new Exception("company name missing");
            }
            this.flight.Company = plan.Company;
            if (plan.InitialLocation["longitude"] < -180 || plan.InitialLocation["longitude"] > 180)
            {
                throw new Exception("longitude range is -180 - +180");
            }
            this.flight.Longitude = plan.InitialLocation["longitude"];
            if (plan.InitialLocation["latitude"] < -90 || plan.InitialLocation["latitude"] > 90)
            {
                throw new Exception("latitude range is -90 - +90");
            }
            this.flight.Latitude = plan.InitialLocation["latitude"];
            this.flight.DateTime = plan.InitialLocation["date_time"];
            this.InitialLocation = plan.InitialLocation;
            this.flight.Is_external = false;
            //hash id
            foreach (Dictionary<string, dynamic> segment in plan.Segments)
            {
                if (segment["latitude"] < -90 || segment["latitude"] > 90)
                {
                    throw new Exception("latitude range is -90 - +90");
                }
                if (segment["longitude"] < -180 || segment["longitude"] > 180)
                {
                    throw new Exception("longitude range is -180 - +180");
                }
                if (segment["timespan_seconds"] < 0)
                {
                    throw new Exception("segment timespan can not be negative");
                }
            }
            this.Segments = plan.Segments;
            this.setFlightId();
            SetEndTime();
        }

        public void UpdateLocation(DateTime time)
        {
            int i = 0;
            DateTime tempTime = this.InitialLocation["date_time"];
            foreach (Dictionary<string, dynamic> segment in Segments)
            {
                if (DateTime.Compare(time, tempTime) > 0)
                {
                    tempTime = tempTime.AddSeconds(segment["timespan_seconds"]);
                    i++;
                }
                else
                {
                    break;
                }
            }
            //calculate how much progress the plane has made in it's current segment:
            double relativeProgress = 1 - ((tempTime - time).TotalSeconds / Segments[i - 1]["timespan_seconds"]);
            // first route segment
            if (i == 1)
            {
                flight.Latitude = (1 - relativeProgress) * this.InitialLocation["latitude"] + relativeProgress * Segments[i - 1]["latitude"];
                flight.Longitude = (1 - relativeProgress) * this.InitialLocation["longitude"] + relativeProgress * Segments[i - 1]["longitude"];
            }
            else // not first route segments
            {
                flight.Latitude = (1 - relativeProgress) * Segments[i - 2]["latitude"] + relativeProgress * Segments[i - 1]["latitude"];
                flight.Longitude = (1 - relativeProgress) * Segments[i - 2]["longitude"] + relativeProgress * Segments[i - 1]["longitude"];
            }
        }

        private void SetEndTime()
        {
            DateTime tempTime = DateTime;
            foreach (Dictionary<string, dynamic> segment in Segments)
            {
                tempTime = tempTime.AddSeconds(segment["timespan_seconds"]);
            }
            EndTime = tempTime;
        }

        public Flight getFlight()
        {
            return this.flight;
        }

        public dynamic getFlightPlanJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        private void setFlightId()
        {
            string toHash = this.Company + this.DateTime.ToString() + this.Longitude.ToString();
            int code = toHash.GetHashCode();
            this.flight.Id = code.ToString("X8");
        }

        private DateTime parseDateTime(string time)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(time, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new Exception("date_time format is incorrect");
            }
            char[] delimeters = { ':', 'T', 'Z', '-' };
            string[] words = time.Split(delimeters);
            return new DateTime(Int32.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2]),
                Int32.Parse(words[3]), Int32.Parse(words[4]), Int32.Parse(words[5]));
        }
    }
}