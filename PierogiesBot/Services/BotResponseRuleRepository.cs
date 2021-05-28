using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PierogiesBot.Models;

namespace PierogiesBot.Services
{
    public class BotResponseRuleRepository : Repository<BotResponseRule>
    {
        public BotResponseRuleRepository(IMongoClient client, ILogger<BotResponseRuleRepository> logger) : base(client, logger)
        {
        }
    }
}