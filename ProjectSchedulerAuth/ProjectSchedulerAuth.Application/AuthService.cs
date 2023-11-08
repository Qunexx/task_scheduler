using ProjectSchedulerAuth.Application.Dtos;
using ProjectSchedulerAuth.Application.Interfaces;
using ProjectSchedulerDomain.Entities;
using System.Security.Cryptography;

namespace ProjectSchedulerAuth.Application
{
    public class AuthService : IAuthService
    {
        public User FilledUserByHashPassword(UserDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSault);

            User user = new User();
            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSault = passwordSault;

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSault)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSault = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}