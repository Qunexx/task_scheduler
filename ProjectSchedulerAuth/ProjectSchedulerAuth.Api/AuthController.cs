using Microsoft.AspNetCore.Mvc;
using ProjectSchedulerAuth.Application.Dtos;
using ProjectSchedulerAuth.Application.Exceptions;
using ProjectSchedulerAuth.Application.Interfaces;
using ProjectSchedulerDomain.Entities;

namespace ProjectSchedulerAuthApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            try
            {
                return Ok(_authService.FilledUserByHashPassword(request));
            }
            catch (RegistrationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}