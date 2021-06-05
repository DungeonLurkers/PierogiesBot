using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BotCrontabRuleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<BotCrontabRuleController> _logger;
        private readonly IRepository<BotCrontabRule> _repository;

        public BotCrontabRuleController(IMapper mapper, ILogger<BotCrontabRuleController> logger, IRepository<BotCrontabRule> repository)
        {
            _mapper = mapper;
            _logger = logger;
            _repository = repository;
        }
        // GET: api/BotCrontabRule
        [HttpGet]
        public async Task<IEnumerable<GetBotCrontabRuleDto>> Get()
        {
            _logger.LogTrace("{0} BotCrontabRules", nameof(Get));
            var entities = await _repository.GetAll();
            return entities.Select(x => _mapper.Map<GetBotCrontabRuleDto>(x));
        }

        // GET: api/BotCrontabRule/5
        [HttpGet("{id}", Name = "GetCrontabRuleById")]
        public async Task<IActionResult> GetCrontabRuleById(string id)
        {
            _logger.LogTrace("{0}: CrontabRule id = {1}", nameof(GetCrontabRuleById), id);
            var rule = await _repository.GetByIdAsync(id);
            return rule is null ? NotFound(id) : Ok(_mapper.Map<GetBotCrontabRuleDto>(rule));
        }

        // POST: api/BotCrontabRule
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateBotCrontabRuleDto ruleDto)
        {
            _logger.LogTrace("{0} BotCrontabRule", "Create");
            try
            {
                var (isEmoji, crontab, replyMessage, replyEmoji, responseMode) = ruleDto;
                var rule = new BotCrontabRule(isEmoji, crontab, replyMessage, replyEmoji, responseMode);
                await _repository.InsertAsync(rule);

                return Ok();
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
                var rule = await _repository.GetByIdAsync(id);

                switch (rule)
                {
                    case null:
                        return NotFound(id);
                    default:
                    {
                        var (isEmoji, crontab, replyMessages, replyEmojis, responseMode) = ruleDto;
                        var updatedRule = rule with
                        {
                            IsEmoji = isEmoji,
                            Crontab = crontab,
                            ReplyMessages = replyMessages,
                            ReplyEmoji = replyEmojis,
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

        // DELETE: api/BotCrontabRule/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogTrace("{0}: CrontabRule id = {1}", "Delete", id);
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