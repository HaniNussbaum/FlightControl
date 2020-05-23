using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace FlightControlWeb.Models
{
    public class Server { 
        public int ServerId { get; set; }
        public string ServerURL { get; set; }

    }

}