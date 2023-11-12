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

        /// <summary>
        /// Регистрация
        /// </summary>
        /// <param name="request">ДТО пользователя</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<ActionResult<User>> Register([FromBody] UserDto request)
        {
            try
            {
                return Ok(await _authService.CreateUser(request));
            }
            catch (RegistrationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Логинизация с возвращением токена
        /// </summary>
        /// <param name="request">ДТО пользователя</param>
        /// <returns></returns> 
        [HttpPost("[action]")]
        public async Task<ActionResult<User>> Login([FromBody] UserDto request)
        {
            try
            {
                return Ok(_authService.LoginizeUser(request));
            }
            catch (RegistrationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}