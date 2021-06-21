using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using PierogiesBot.GrainsInterfaces;
using PierogiesBot.GrainsInterfaces.Discord;

// ReSharper disable VSTHRD200

namespace PierogiesBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuildsController : ControllerBase
    {
        private readonly IClusterClient _client;
        private readonly ILogger<GuildsController> _logger;

        public GuildsController(IClusterClient client, ILogger<GuildsController> logger)
        {
            _client = client;
            _logger = logger;
        }

        [HttpGet(Name = "GetAllGuilds")]
        public async Task<IActionResult> GetAllGuilds()
        {
            _logger.LogTrace("{0} invoked", nameof(GetAllGuilds));
            var grain = _client.GetGrain<IDiscordGuildGrain>(HttpContext.TraceIdentifier);

            return Ok(await grain.GetGuildsAsync());
        }

        [HttpGet("{id}", Name = "GetGuildById")]
        public async Task<IActionResult> GetGuildById(ulong id)
        {
            _logger.LogTrace("{0} invoked", nameof(GetGuildById));
            var grain = _client.GetGrain<IDiscordGuildGrain>(HttpContext.TraceIdentifier);

            return Ok(await grain.GetGuildByIdAsync(id));
        }
    }
}