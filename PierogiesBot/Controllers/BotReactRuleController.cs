using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PierogiesBot.Models;
using PierogiesBot.Models.Dtos.BotReactRule;
using PierogiesBot.Models.Dtos.BotResponseRule;
using PierogiesBot.Services;

namespace PierogiesBot.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BotReactRuleController : ControllerBase
    {
        private readonly ILogger<BotReactRuleController> _logger;
        private readonly IRepository<BotReactRule> _repository;

        public BotReactRuleController(ILogger<BotReactRuleController> logger, IRepository<BotReactRule> repository)
        {
            _logger = logger;
            _repository = repository;
        }
        // GET: api/BotResponseRule
        [HttpGet]
        public async Task<IEnumerable<BotReactRule>> Get()
        {
            _logger.LogTrace("{0}", nameof(Get));
            return await _repository.GetAll();
        }

        // GET: api/BotResponseRule/5
        [HttpGet("{id}", Name = "GetReactRuleById")]
        public async Task<IActionResult> GetReactRuleById(string id)
        {
            _logger.LogTrace("{0}: Rule id = {1}", nameof(GetReactRuleById), id);
            var rule = await _repository.GetByIdAsync(id);
            return rule is null ? NotFound(id) : Ok(rule);
        }

        // POST: api/BotResponseRule
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateBotReactRuleDto ruleDto)
        {
            _logger.LogTrace("{0}", nameof(Post));
            try
            {
                var (reaction, triggerText, stringComparison, isTriggerTextRegex, shouldTriggerOnContains) = ruleDto;
                var rule = new BotReactRule(reaction, triggerText, stringComparison, isTriggerTextRegex,
                    shouldTriggerOnContains);
                await _repository.InsertAsync(rule);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // PUT: api/BotResponseRule/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateBotReactRuleDto ruleDto)
        {
            _logger.LogTrace("{0}: Rule id = {1}", nameof(Put), id);
            try
            {
                var rule = await _repository.GetByIdAsync(id);

                switch (rule)
                {
                    case null:
                        return NotFound(id);
                    default:
                    {
                        var (reaction, triggerText, stringComparison, isTriggerTextRegex, shouldTriggerOnContains) = ruleDto;
                        var updatedRule = rule with
                        {
                            Reaction = reaction, 
                            TriggerText = triggerText, 
                            StringComparison = stringComparison, 
                            IsTriggerTextRegex = isTriggerTextRegex, 
                            ShouldTriggerOnContains = shouldTriggerOnContains
                        };
                        
                        await _repository.UpdateAsync(updatedRule);
                        
                        return Ok();
                    }
                }
            }
            catch (Exception e)
            {

                return BadRequest(e);
            }
        }

        // DELETE: api/BotResponseRule/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogTrace("{0}: Rule id = {1}", nameof(Delete), id);
            try
            {
                var user = await _repository.GetByIdAsync(id);

                switch (user)
                {
                    case null:
                        return NotFound(id);
                    default:
                    {
                        await _repository.DeleteAsync(id);
                        
                        return Ok();
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
