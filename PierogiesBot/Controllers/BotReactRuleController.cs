using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BotReactRuleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<BotReactRuleController> _logger;
        private readonly IRepository<BotReactRule> _repository;

        public BotReactRuleController(IMapper mapper, ILogger<BotReactRuleController> logger, IRepository<BotReactRule> repository)
        {
            _mapper = mapper;
            _logger = logger;
            _repository = repository;
        }
        // GET: api/BotResponseRule
        [HttpGet]
        public async Task<IEnumerable<GetBotReactRuleDto>> Get()
        {
            _logger.LogTrace("{0} BotReactRule", nameof(Get));
            var entities =  await _repository.GetAll();

            return entities.Select(x => _mapper.Map<GetBotReactRuleDto>(x));
        }

        // GET: api/BotResponseRule/5
        [HttpGet("{id}", Name = "GetReactRuleById")]
        public async Task<IActionResult> GetReactRuleById(string id)
        {
            _logger.LogTrace("{0}: ReactRule id = {1}", nameof(GetReactRuleById), id);
            var rule = await _repository.GetByIdAsync(id);
            return rule is null ? NotFound(id) : Ok(_mapper.Map<GetBotReactRuleDto>(rule));
        }

        // POST: api/BotResponseRule
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateBotReactRuleDto ruleDto)
        {
            _logger.LogTrace("{0} ReactRule", "Create");
            try
            {
                var (reaction, triggerText, stringComparison, isTriggerTextRegex, shouldTriggerOnContains, responseMode) = ruleDto;
                var rule = new BotReactRule(reaction, triggerText, stringComparison, isTriggerTextRegex,
                    shouldTriggerOnContains, responseMode);
                await _repository.InsertAsync(rule);

                return Ok(rule.Id);
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
            _logger.LogTrace("{0}: ReactRule id = {1}", nameof(Put), id);
            try
            {
                var rule = await _repository.GetByIdAsync(id);

                switch (rule)
                {
                    case null:
                        return NotFound(id);
                    default:
                    {
                        var (reactions, triggerText, stringComparison, isTriggerTextRegex, shouldTriggerOnContains, responseMode) = ruleDto;
                        var updatedRule = rule with
                        {
                            Reactions = reactions, 
                            TriggerText = triggerText, 
                            StringComparison = stringComparison, 
                            IsTriggerTextRegex = isTriggerTextRegex, 
                            ShouldTriggerOnContains = shouldTriggerOnContains,
                            ResponseMode = responseMode
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
            _logger.LogTrace("{0}: ReactRule id = {1}", nameof(Delete), id);
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
