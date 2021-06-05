using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PierogiesBot.Commons.Dtos.UserData;
using PierogiesBot.Models;
using PierogiesBot.Settings;

namespace PierogiesBot.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<MongoIdentityRole> _roleManager;
        private IConfiguration _configuration;
        private JwtSettings _jwtSettings;

        public UserController(ILogger<UserController> logger, UserManager<AppUser> userManager, RoleManager<MongoIdentityRole> roleManager,
            IConfiguration configuration, IOptions<JwtSettings> jwtOptions)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            
            _jwtSettings = jwtOptions.Value;
        }

        [Authorize(Roles = "user,admin")]
        [HttpGet]
        public IActionResult GetHello() => Ok("Hello");

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
        {
            _logger.LogInformation("Authenticating user {0}", request.UserName);
            var (userName, password) = request;
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null || !await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogInformation("Authorization of user {0} failed!", request.UserName);
                return Unauthorized();
            }
            var utcNow = DateTime.UtcNow;

            var authClaims = user.Claims.Select(c => new Claim(c.Type, c.Value));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            var expires = DateTime.UtcNow.AddDays(3);
            var token = new JwtSecurityToken(
                expires: expires,
                notBefore: utcNow,
                audience: _jwtSettings.ValidAudience,
                issuer: _jwtSettings.ValidIssuer,
                claims: authClaims,  
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)  
            );

            _logger.LogInformation("Authorization of user {request.UserName} is successful. Returning JWT");
                
            return Ok(new AuthenticateResponse(new JwtSecurityTokenHandler().WriteToken(token), user.Id, userName));
        }
        
        // GET: api/User/5
        [HttpGet("{userName}", Name = "GetUserByUserName")]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            _logger.LogTrace("{0}: UserName = {1}", nameof(GetUserByUserName), userName);
            var user = await _userManager.FindByNameAsync(userName);
            return user is null ? NotFound(userName) : Ok(user);
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserDto userDto)
        {
            _logger.LogTrace("{0} User", "Create");
            try
            {
                var (userName,email, password, roles) = userDto;
                var user = new AppUser()
                {
                    UserName = userName,
                    Email = email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    user = await _userManager.FindByNameAsync(userName);
                    var roleList = roles.ToList();
                    if (!roleList.Any()) roleList.Add("user");
                    foreach (var role in roleList)
                    {
                        if (await _roleManager.RoleExistsAsync(role))
                        {
                            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }

                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, userName));

                    return Ok(new {UserName = userName, Email = email, Roles = roleList});
                }
                else
                    return BadRequest(new {Errors = result.Errors.Select(e => e.Description)});
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateUserDto userDto)
        {
            _logger.LogTrace("{0}: User id = {1}", "Update", id);
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                switch (user)
                {
                    case null:
                        _logger.LogInformation("User to update not found! Id = {0}", id);
                        return NotFound(id);
                    default:
                    {
                        var (userName, email, roles) = userDto;
                        _logger.LogInformation("Found user {0} to update", userName);

                        if (userName != null) user.UserName = userName;
                        if (email != null) user.Email = email;
                        if (roles != null) user.Roles = roles.ToList();

                        await _userManager.UpdateAsync(user);
                        
                        return Ok();
                    }
                }
            }
            catch (Exception e)
            {

                return BadRequest(e);
            }
        }

        // DELETE: api/User/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogTrace("{0}: User id = {1}", nameof(Delete), id);
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                switch (user)
                {
                    case null:
                        _logger.LogInformation("Not found user to remove. User Id = {0}", id);
                        return NotFound(id);
                    default:
                    {
                        _logger.LogInformation("User to remove found. UserName = {0}, UserId = {1}", user.UserName, id);
                        await _userManager.DeleteAsync(user);
                        _logger.LogInformation("User removed. User Id = {0}", id);
                        return Ok();
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
