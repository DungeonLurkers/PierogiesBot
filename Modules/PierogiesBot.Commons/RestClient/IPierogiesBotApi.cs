using System.Net.Http.Headers;
using System.Threading.Tasks;
using PierogiesBot.Commons.Dtos.UserData;
using RestEase;

namespace PierogiesBot.Commons.RestClient
{
    public interface IPierogiesBotApi
    {
        [Header("Authorization")]
        AuthenticationHeaderValue AuthenticationHeaderValue {get; set; }

        [Post("/api/User/auth")]
        public Task<AuthenticateResponse> Authenticate([Body] AuthenticateRequest request);

        [Get("/api/User/{id}")]
        Task<GetUserDto> GetUser([Path] string id);
    }
}