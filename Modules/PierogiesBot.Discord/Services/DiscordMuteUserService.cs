using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.Mute;
using PierogiesBot.Data.Extensions;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Discord.Services
{
    public class DiscordMuteUserService : IDiscordMuteUserService
    {
        private readonly IRepository<Mute> _muteRepository;
        private readonly ISettingsService _settingsService;
        private readonly ILogger<DiscordMuteUserService> _logger;
        private readonly IMapper _mapper;

        public DiscordMuteUserService(IRepository<Mute> muteRepository, ISettingsService settingsService, ILogger<DiscordMuteUserService> logger, IMapper mapper)
        {
            _muteRepository = muteRepository;
            _settingsService = settingsService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetMuteDto>> GetAllMutes()
        {
            var mutes = await _muteRepository.GetAll();
            var mapped = mutes.Select(x => _mapper.Map<GetMuteDto>(x)).ToList();

            return mapped;
        }

        public async Task<GetMuteDto?> GetMuteForUser(SocketGuildUser user)
        {
            var mute = await _muteRepository.FindByDiscordUserId(user.Id);

            var mapped = _mapper.Map<GetMuteDto>(mute);

            return mapped;
        }

        public async Task MuteUser(SocketGuildUser user, CreateMuteDto dto)
        {
            _logger.LogInformation($"Muting user {user} in guild {user.Guild}");
            var muteRole = await _settingsService.GetMuteRole(user.Guild.Id);
            if (muteRole is null) return;
            _logger.LogTrace($"Using mute role {muteRole}");

            var userId = user.Id;

            _logger.LogTrace("Saving mute to database");
            await SaveMute(userId, dto);

            var userRoles = user.Roles;
            _logger.LogTrace($"Removing all roles ({userRoles.Count}) from {user}");
            await user.RemoveRolesAsync(userRoles.Where(x => !x.IsEveryone));

            _logger.LogTrace($"Adding mute role {muteRole} to user {user}");
            await user.AddRoleAsync(muteRole);

            _logger.LogInformation($"Muted {user} until {dto.Until:F} because \"{dto.Reason}\"");
        }

        public async Task UnmuteUser(SocketGuildUser user)
        {
            _logger.LogInformation($"Unmuting user {user}");
            var mute = await _muteRepository.FindByDiscordUserId(user.Id);

            if (mute is not null)
            {
                _logger.LogTrace($"Found mute for user {user}");
                var roles = user.Guild.Roles.Where(x => mute.RolesIds.Contains(x.Id) && !x.IsEveryone).ToList();
                
                _logger.LogTrace($"Removing mute role for user {user}");
                await user.RemoveRolesAsync(user.Roles.Where(x => !x.IsEveryone));

                _logger.LogTrace($"Restoring roles ({roles.Count}) for user {user}");
                await user.AddRolesAsync(roles);

                await DeleteMute(mute.Id);
            }
        }

        private async Task SaveMute(ulong userId, CreateMuteDto dto)
        {
            _logger.LogDebug($"Saving mute for user with id {dto.DiscordUserId}");
            var mute = await _muteRepository.FindByDiscordUserId(userId);

            if (mute is null)
            {
                mute = _mapper.Map<Mute>(dto);
                await _muteRepository.InsertAsync(mute);
            }
            else
            {
                mute = _mapper.Map<Mute>(dto) with { Id = mute.Id };
                await _muteRepository.UpdateAsync(mute);
            }
        }

        private async Task DeleteMute(string id) => await _muteRepository.DeleteAsync(id);
    }
}