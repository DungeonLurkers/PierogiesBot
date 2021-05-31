using System.Security;
using System.Threading.Tasks;
using PierogiesBot.Commons.Dtos.UserData;
using RestEase;

namespace PierogiesBot.Manager.Services
{
    public interface IPierogiesBotService
    {
        bool IsAuthenticated {get;}
        string? Token {get;}

        Task<bool> Authenticate(string userName, SecureString password);

        Task<GetUserDto?> GetUserData(string userName);
    }
}