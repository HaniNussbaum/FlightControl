using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Server
        
    {
        private int serverIndex;
        private static IMemoryCache _cache;
        public Server(IMemoryCache cache)
        {
            _cache = cache;
        }
        public Dictionary<int, JsonResult> getServersInstance()
        {
            var servers = new Dictionary<int, JsonResult>();
            if (!_cache.TryGetValue("ServerList", out servers))
            {
                if (servers == null)
                {
                    servers = new Dictionary<int, JsonResult>();
                }
                _cache.Set("ServerList", servers);
            }
            return servers;
        }
        public void AddServer(JsonResult server)
        {
            var servers = getServersInstance();
            servers.Add(servers.Count, server);
        }
            public void DeleteServer(int ID)
        {
            
        }
    }

}