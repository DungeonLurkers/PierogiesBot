using AutoMapper;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Commons.Dtos.BotResponseRule;
using PierogiesBot.Commons.Dtos.Mute;
using PierogiesBot.Commons.Dtos.UserData;
using PierogiesBot.Data.Models;
using PierogiesBot.Models;

namespace PierogiesBot
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<BotResponseRule, GetBotResponseRuleDto>().ReverseMap();
            CreateMap<BotResponseRule, CreateBotResponseRuleDto>().ReverseMap();
            CreateMap<BotResponseRule, UpdateBotResponseRuleDto>().ReverseMap();

            CreateMap<BotReactRule, GetBotReactRuleDto>().ReverseMap();
            CreateMap<BotReactRule, CreateBotReactRuleDto>().ReverseMap();
            CreateMap<BotReactRule, UpdateBotReactRuleDto>().ReverseMap();

            CreateMap<BotCrontabRule, GetBotCrontabRuleDto>().ReverseMap();
            CreateMap<BotCrontabRule, CreateBotCrontabRuleDto>().ReverseMap();
            CreateMap<BotCrontabRule, UpdateBotCrontabRuleDto>().ReverseMap();

            CreateMap<AppUser, GetUserDto>().ReverseMap();
            CreateMap<AppUser, UpdateUserDto>().ReverseMap();
            CreateMap<AppUser, CreateUserDto>().ReverseMap();

            CreateMap<Mute, GetMuteDto>().ReverseMap();
            CreateMap<Mute, UpdateMuteDto>().ReverseMap();
            CreateMap<Mute, CreateMuteDto>().ReverseMap();
        }
    }
}