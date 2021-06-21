using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PierogiesBot.Manager.Models.Entities;

namespace PierogiesBot.Manager.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<SettingsService> _logger;

        public SettingsService(AppDbContext dbContext, ILogger<SettingsService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Task Set(string userName = "", string token = "")
        {
            return Set(settings =>
             {
                 settings.CurrentUserName = userName;
                 settings.ApiToken = token;
             });
        }

        public async Task Set(Action<Settings> configure)
        {
            var existing = await Get().ConfigureAwait(false);

            if (existing is not null)
            {
                configure(existing);
                _dbContext.Update(existing);
            }
            else
            {
                var settings = new Settings("", "");

                configure(settings);

                await _dbContext.Settings.AddAsync(settings).ConfigureAwait(false);
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<Settings?> Get()
        {
            return Request(async ctx => await ctx.Settings.SingleOrDefaultAsync().ConfigureAwait(false));
        }

        public Task<string?> GetToken()
        {
            return Request(async ctx => await ctx.Settings.Select(x => x.ApiToken).SingleOrDefaultAsync().ConfigureAwait(false));
        }

        private async Task<T?> Request<T>(Func<AppDbContext, Task<T>> func, [CallerMemberName] string? callerMemberName = null)
        {
            try
            {
                return await func(_dbContext).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while {0}", callerMemberName ?? "Request");
                return default;
            }
        }
    }
}