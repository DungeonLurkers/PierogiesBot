using System.Security;
using System.Threading.Tasks;

namespace PierogiesBot.Manager.Services
{
    public interface IPierogiesBotService
    {
        bool IsAuthenticated {get;}
        string? Token {get;}

        Task<bool> Authenticate(string userName, SecureString password);
    }
}