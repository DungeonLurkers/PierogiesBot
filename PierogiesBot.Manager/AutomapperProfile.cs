using AutoMapper;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Manager.Models;

namespace PierogiesBot.Manager
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<ResponseRuleModel, GetBotResponseRuleDto>().ReverseMap();
            CreateMap<ReactionRuleModel, GetBotReactRuleDto>().ReverseMap();
            CreateMap<CrontabRuleModel, GetBotCrontabRuleDto>().ReverseMap();
        }
    }
}