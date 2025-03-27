using API.Supports.Interfaces;
using Core.BizLogics;
using Database.Models;
using Database.Supports.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtAuthenticationManager _authenticationManager;
        private readonly ILogger<AuthController> _logger;
        protected UsersBizLogic BizLogic { get; }

        public AuthController(IJwtAuthenticationManager authenticationManager, ILogger<AuthController> logger, PopAppContext context, IConfiguration configuration)
        {
            _authenticationManager = authenticationManager;
            _logger = logger;
            BizLogic = new UsersBizLogic(configuration, context);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserModel userCredentials)
        {
            try
            {
                if (userCredentials != null)
                {
                    var user = await BizLogic.GetOrInsertUser(userCredentials);
                    if (user != null && new PasswordHasher<string>().VerifyHashedPassword(userCredentials.UserName, user.Password, userCredentials.Password) == PasswordVerificationResult.Success)
                    {
                        var token = _authenticationManager.Authenticate(user.Id, user.UserName);
                        if (string.IsNullOrWhiteSpace(token))
                        {
                            throw new Exception("Unauthorized");
                        }
                        return Ok(new AuthenticationModel()
                        {
                            Token = token
                        });
                    }
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return Unauthorized();
            }
        }
    }
}
