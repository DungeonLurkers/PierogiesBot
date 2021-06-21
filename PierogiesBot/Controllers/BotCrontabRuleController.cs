using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.GrainsInterfaces;
using PierogiesBot.GrainsInterfaces.Data;

namespace PierogiesBot.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BotCrontabRuleController : ControllerBase
    {
        private readonly ILogger<BotCrontabRuleController> _logger;
        private readonly IClusterClient _clusterClient;

        public BotCrontabRuleController(ILogger<BotCrontabRuleController> logger, IClusterClient clusterClient)
        {
            _logger = logger;
            _clusterClient = clusterClient;
        }

        // GET: api/BotCrontabRule
        [HttpGet]
        public async Task<IEnumerable<GetBotCrontabRuleDto>> Get()
        {
            _logger.LogTrace("{0} BotCrontabRules", nameof(Get));
            var grain = _clusterClient.GetGrain<IBotCrontabRuleGrain>(HttpContext.TraceIdentifier);

            var rules = await grain.Find();

            return rules;
        }

        // GET: api/BotCrontabRule/5
        [HttpGet("{id}", Name = "GetCrontabRuleById")]
        public async Task<IActionResult> GetCrontabRuleById(string id)
        {
            _logger.LogTrace("{0}: CrontabRule id = {1}", nameof(GetCrontabRuleById), id);
            var grain = _clusterClient.GetGrain<IBotCrontabRuleGrain>(HttpContext.TraceIdentifier);
            var rule = await grain.FindById(id);
            return rule is null ? NotFound(id) : Ok(rule);
        }

        // POST: api/BotCrontabRule
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateBotCrontabRuleDto ruleDto)
        {
            _logger.LogTrace("{0} BotCrontabRule", "Create");
            try
            {
                var grain = _clusterClient.GetGrain<IBotCrontabRuleGrain>(HttpContext.TraceIdentifier);

                var id = await grain.Create(ruleDto);
                return Ok(new { Id = id });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // PUT: api/BotCrontabRule/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateBotCrontabRuleDto ruleDto)
        {
            _logger.LogTrace("{0}: CrontabRule id = {1}", "Update", id);
            try
            {
                var grain = _clusterClient.GetGrain<IBotCrontabRuleGrain>(HttpContext.TraceIdentifier);

                var result = await grain.Update(id, ruleDto);

                return result is "" ? NotFound() : Ok(new { Id = id });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // DELETE: api/BotCrontabRule/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogTrace("{0}: CrontabRule id = {1}", "Delete", id);
            try
            {
                var grain = _clusterClient.GetGrain<IBotCrontabRuleGrain>(HttpContext.TraceIdentifier);

                var result = await grain.Delete(id);
                return result is "" ? NotFound() : Ok(new { Id = id });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}