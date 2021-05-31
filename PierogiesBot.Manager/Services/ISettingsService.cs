using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PierogiesBot.Manager.Models.Entities;

namespace PierogiesBot.Manager.Services
{
    public interface ISettingsService
    {
        Task Set(Settings settings);
        Task Set(string userName = "", string token = "");

        Task<Settings?> Get();
        
        Task<string> GetToken();
    }

    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _dbContext;

        public SettingsService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Set(string userName = "", string token = "") => await Set(new Settings(userName, token));

        public async Task Set(Settings settings)
        {
            var existing = await Get();
            
            if (existing is not null) _dbContext.Settings.Update(settings with {Id = existing.Id});
            else await _dbContext.Settings.AddAsync(settings);
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Settings?> Get() => await _dbContext.Settings.SingleOrDefaultAsync();
        public async Task<string> GetToken() => await _dbContext.Settings.Select(x => x.ApiToken).SingleOrDefaultAsync();
    }
}