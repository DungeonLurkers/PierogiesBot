namespace PierogiesBot.Models.Dtos.UserData
{
    public record AuthenticateResponse(string Token, string Id, string UserName);
}