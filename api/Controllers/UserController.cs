using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.DTOS;
using api.DTOS.Requests;
using api.DTOS.Response;
using api.Repositories;
using api.Repositories.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private const string jwtKey = "JwtKey";
        private readonly AppDBContext Context;
        private readonly IConfiguration Configuration;

        public UserController(AppDBContext context, IConfiguration configuration)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // Create Token
        private LoginResponsesDTO Token(UserDTO user)
        {
            try
            {
                DateTime expires = DateTime.UtcNow.AddHours(16);
                List<Claim> claims = new() { new Claim("SystemName", user.SystemName) };

                // Ensure the key is long enough (at least 16 bytes)
                string? keyString = Configuration[jwtKey];
                if (keyString == null || keyString.Length < 16)
                {
                    throw new InvalidOperationException("JWT key is not long enough.");
                }

                SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(keyString));
                SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);
                JwtSecurityToken securityToken =
                    new(
                        issuer: null,
                        audience: null,
                        claims: claims,
                        expires: expires,
                        signingCredentials: credentials
                    );

                return new LoginResponsesDTO
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(securityToken)
                };
            }
            catch (Exception ex)
            {
                return new LoginResponsesDTO { Token = "Error generating token." + ex.Message };
            }
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        [HttpPost("registration")]
        public async ValueTask<ActionResult<LoginResponsesDTO>> Create(
            [FromBody] LoginRequestDTO body
        )
        {
            try
            {
                if (body == null)
                {
                    return BadRequest("Request body is null.");
                }

                if (await Context.Users.AnyAsync(u => u.Username.Trim() == body.Username.Trim()))
                {
                    return BadRequest("Username already exists.");
                }

                string hashedPassword = HashPassword(body.Password);

                User userObj = new() { Username = body.Username, Password = hashedPassword };
                await Context.Users.AddAsync(userObj);
                await Context.SaveChangesAsync();

                UserDTO newUserDTO =
                    new()
                    {
                        Id = userObj.Id,
                        SystemName = userObj.Username,
                        Password = "x" // Ideally, don't send the password back
                    };

                return Ok(Token(newUserDTO));
            }
            catch (Exception ex)
            {
                string msg = $"Error creating user: {ex.Message}, StackTrace: {ex.StackTrace}";
                return BadRequest(msg);
            }
        }
    }
}
