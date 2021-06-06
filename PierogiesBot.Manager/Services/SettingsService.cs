using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PierogiesBot.Manager.Models.Entities;

namespace PierogiesBot.Manager.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _dbContext;

        public SettingsService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Set(string userName = "", string token = "")
        {
            await Set(settings =>
            {
                settings.CurrentUserName = userName;
                settings.ApiToken = token;
            });
        }

        public async Task Set(Action<Settings> configure)
        {
            var existing = await Get();

            if (existing is not null)
            {
                configure(existing);
                _dbContext.Update(existing);
            }
            else
            {
                var settings = new Settings("", "");

                configure(settings);

                await _dbContext.Settings.AddAsync(settings);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Settings?> Get()
        {
            return await _dbContext.Settings.SingleOrDefaultAsync();
        }

        public async Task<string> GetToken()
        {
            return await _dbContext.Settings.Select(x => x.ApiToken).SingleOrDefaultAsync();
        }
    }
}