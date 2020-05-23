using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using FlightControlWeb.Models;

namespace FlightControlWeb.Controllers
{
    [Route("api/servers")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private IMemoryCache _cache;
        public ServerController(IMemoryCache cache)
        {
            _cache = cache;
        }
        [HttpGet]
        public List<Server> GetServers()
        {
            var serversList = new List<Server>();
            if (!_cache.TryGetValue("ServerList", out serversList))
            {
                if (serversList == null)
                {
                    return null;
                }
            }
            foreach(Server server in serversList)
            {
                System.Diagnostics.Debug.WriteLine(server.ServerURL);
            }
            return serversList;
        }
        [HttpPost]
        public ActionResult Post([FromBody] Server currServer)
        {
            var serversDictionary = new Dictionary<int, Server>();
            var serversList = new List<Server>();
            if (!_cache.TryGetValue("ServerDictionary", out serversDictionary))
            {
                if (serversDictionary == null)
                {
                    Console.WriteLine("dictionarynull");
                    serversDictionary = new Dictionary<int, Server>();
                }
                _cache.Set("ServerList", serversDictionary);
            }
            if (!_cache.TryGetValue("ServerList", out serversList))
            {
                if (serversList == null)
                {
                    serversList = new List<Server>();
                    System.Diagnostics.Debug.WriteLine("serversnull");
                }
                _cache.Set("ServerList", serversList);
            }
            int currServerId = currServer.ServerId;
            System.Diagnostics.Debug.WriteLine(currServerId);
            serversDictionary.Add(currServerId, currServer);
            serversList.Add(currServer);
            // return CreatedAtAction(actionName: "GetServer", new { id = currServer.ServerId }, currServer);
            return Ok(currServerId);
        }

        //return list of servers in json format
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var serversDictionary = new Dictionary<int, Server>();
            var serversList = new List<Server>();
            if (!_cache.TryGetValue("ServerDictionary", out serversDictionary))
            {
                if (serversDictionary == null)
                {
                    return BadRequest();
                }

            }
            if (!_cache.TryGetValue("ServerList", out serversList))
            {
                if (serversList == null)
                {
                    return BadRequest();
                }

            }
            if (serversDictionary.ContainsKey(id))
                {
                    Server currServer = serversDictionary[id];
                    serversList.Remove(currServer);
                    serversDictionary.Remove(id);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
          
        }

    }
}