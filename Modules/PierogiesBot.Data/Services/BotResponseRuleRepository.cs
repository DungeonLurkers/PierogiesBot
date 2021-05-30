using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PierogiesBot.Data.Models;

namespace PierogiesBot.Data.Services
{
    public class BotResponseRuleRepository : Repository<BotResponseRule>
    {
        public BotResponseRuleRepository(IMongoClient client, ILogger<BotResponseRuleRepository> logger,
            IMediator mediator) : base(mediator, client, logger)
        {
        }
    }
}