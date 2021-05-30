using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BotCrontabRuleController : ControllerBase
    {
        private readonly ILogger<BotCrontabRuleController> _logger;
        private readonly IRepository<BotCrontabRule> _repository;

        public BotCrontabRuleController(ILogger<BotCrontabRuleController> logger, IRepository<BotCrontabRule> repository)
        {
            _logger = logger;
            _repository = repository;
        }
        // GET: api/BotCrontabRule
        [HttpGet]
        public async Task<IEnumerable<BotCrontabRule>> Get()
        {
            _logger.LogTrace("{0}", nameof(Get));
            return await _repository.GetAll();
        }

        // GET: api/BotCrontabRule/5
        [HttpGet("{id}", Name = "GetCrontabRuleById")]
        public async Task<IActionResult> GetCrontabRuleById(string id)
        {
            _logger.LogTrace("{0}: Rule id = {1}", nameof(GetCrontabRuleById), id);
            var rule = await _repository.GetByIdAsync(id);
            return rule is null ? NotFound(id) : Ok(rule);
        }

        // POST: api/BotCrontabRule
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateBotCrontabRuleDto ruleDto)
        {
            _logger.LogTrace("{0}", nameof(Post));
            try
            {
                var (isEmoji, crontab, replyMessage, replyEmoji, timeZoneInfo) = ruleDto;
                var rule = new BotCrontabRule(isEmoji, crontab, replyMessage, replyEmoji, timeZoneInfo);
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
                        var (isEmoji, crontab, replyMessages, replyEmojis) = ruleDto;
                        var updatedRule = rule with
                        {
                            IsEmoji = isEmoji,
                            Crontab = crontab,
                            ReplyMessages = replyMessages,
                            ReplyEmoji = replyEmojis,
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