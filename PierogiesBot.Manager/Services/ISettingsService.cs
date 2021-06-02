using System;
using System.Threading.Tasks;
using PierogiesBot.Manager.Models.Entities;

namespace PierogiesBot.Manager.Services
{
    public interface ISettingsService
    {
        Task Set(Action<Settings> configure);
        Task Set(string userName = "", string token = "");

        Task<Settings?> Get();
        
        Task<string> GetToken();
    }
}