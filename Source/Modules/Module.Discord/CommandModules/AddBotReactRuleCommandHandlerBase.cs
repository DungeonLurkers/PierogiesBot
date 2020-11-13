using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Module.Data.Models;
using Module.Data.Storage;

namespace Module.Discord.CommandModules
{
    public class AddBotReactRuleCommandHandlerBase : ModuleBase<SocketCommandContext>
    {
        private readonly IDataSource<BotReactRule, Guid> _botReactRules;

        public AddBotReactRuleCommandHandlerBase(IDataSource<BotReactRule, Guid> botReactRules)
        {
            _botReactRules = botReactRules;
        }
        public async Task AddReactRule(SocketMessage message, BotReactRule rule)
        {
            
        }
    }
}