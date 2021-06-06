using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PierogiesBot.Data.Models;

namespace PierogiesBot.Data.Services
{
    public class BotResponseRuleRepository : Repository<BotResponseRule>
    {
        public BotResponseRuleRepository(IMongoClient client, ILogger<BotResponseRuleRepository> logger,
            IMessageBus messageBus) : base(client, logger, messageBus)
        {
        }
    }
}