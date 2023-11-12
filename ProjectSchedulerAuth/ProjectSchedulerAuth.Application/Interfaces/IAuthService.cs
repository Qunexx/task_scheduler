using ProjectSchedulerAuth.Application.Dtos;
using ProjectSchedulerDomain.Entities;

namespace ProjectSchedulerAuth.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<User> CreateUser(UserDto request);
        public string LoginizeUser(UserDto request);
    }
}