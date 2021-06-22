using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using PierogiesBot.Commons.Dtos;
using PierogiesBot.Commons.Dtos.BotCrontabRule;
using PierogiesBot.Data.Models;

namespace PierogiesBot.GrainsInterfaces.Data
{
    public interface IEntityGrain : IGrainWithStringKey
    {
        Task<string> Delete(string id);
    }
}