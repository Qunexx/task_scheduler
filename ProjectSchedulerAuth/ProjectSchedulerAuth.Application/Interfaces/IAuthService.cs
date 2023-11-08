using ProjectSchedulerAuth.Application.Dtos;
using ProjectSchedulerDomain.Entities;

namespace ProjectSchedulerAuth.Application.Interfaces
{
    public interface IAuthService
    {
        public User FilledUserByHashPassword(UserDto request);
    }
}