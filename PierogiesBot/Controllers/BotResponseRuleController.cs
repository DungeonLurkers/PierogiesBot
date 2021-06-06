using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BotResponseRuleController : ControllerBase
    {
        private readonly ILogger<BotResponseRuleController> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<BotResponseRule> _repository;

        public BotResponseRuleController(IMapper mapper, ILogger<BotResponseRuleController> logger,
            IRepository<BotResponseRule> repository)
        {
            _mapper = mapper;
            _logger = logger;
            _repository = repository;
        }

        // GET: api/BotResponseRule
        [HttpGet]
        public async Task<IEnumerable<GetBotResponseRuleDto>> Get()
        {
            _logger.LogTrace("{0} ResponseRule", nameof(Get));
            var entities = await _repository.GetAll();

            return entities.Select(e => _mapper.Map<GetBotResponseRuleDto>(e));
        }

        // GET: api/BotResponseRule/5
        [HttpGet("{id}", Name = "GetResponseRuleById")]
        public async Task<IActionResult> GetResponseRuleById(string id)
        {
            _logger.LogTrace("{0}: ResponseRule id = {1}", nameof(GetResponseRuleById), id);
            var rule = await _repository.GetByIdAsync(id);
            return rule is null ? NotFound(id) : Ok(_mapper.Map<GetBotResponseRuleDto>(rule));
        }

        // POST: api/BotResponseRule
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateBotResponseRuleDto ruleDto)
        {
            _logger.LogTrace("{0} ResponseRule", "Create");
            try
            {
                var (responseMode, respondWith, triggerText, stringComparison, isTriggerTextRegex,
                    shouldTriggerOnContains) = ruleDto;
                var rule = new BotResponseRule(responseMode, respondWith, triggerText, stringComparison,
                    isTriggerTextRegex,
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
        public async Task<IActionResult> Put(string id, [FromBody] UpdateBotResponseRuleDto ruleDto)
        {
            _logger.LogTrace("{0}: ResponseRule id = {1}", "Update", id);
            try
            {
                var rule = await _repository.GetByIdAsync(id);

                switch (rule)
                {
                    case null:
                        return NotFound(id);
                    default:
                    {
                        var (responseMode, respondWith, triggerText, stringComparison, isTriggerTextRegex,
                            shouldTriggerOnContains) = ruleDto;
                        var updatedRule = rule with
                        {
                            ResponseMode = responseMode,
                            Responses = respondWith,
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