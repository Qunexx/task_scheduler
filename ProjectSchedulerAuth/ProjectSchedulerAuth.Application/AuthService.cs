using Microsoft.IdentityModel.Tokens;
using ProjectSchedulerAuth.Application.Dtos;
using ProjectSchedulerAuth.Application.Exceptions;
using ProjectSchedulerAuth.Application.Interfaces;
using ProjectSchedulerAuth.Domain.Entities;
using ProjectSchedulerAuth.Domain.Repositories;
using ProjectSchedulerAuth.Infrastructure.Tools;
using ProjectSchedulerDomain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ProjectSchedulerAuth.Application
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User, string> _userRepository;
        private readonly Toolbox _toolbox;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IRepository<User, string> userRepository, Toolbox toolbox, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _toolbox = toolbox;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> CreateUser(UserDto request)
        {
            var currentUser = _userRepository.GetByKey(request.SpecialName);
            if (currentUser != null)
            {
                throw new RegistrationException("User already exist");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSault);
            User user = new User(request.Username, request.SpecialName, passwordHash, passwordSault);

            _userRepository.Add(user);
            var saved = await _unitOfWork.CommitAsync();

            if (saved == 0) throw new RegistrationException("Error while saving in db");

            return user;
        }

        public string LoginizeUser(UserDto request)
        {
            var currentUser = _userRepository.GetByKeyWithInclude(x => x.Id.Equals(request.SpecialName), p => p.UserRoles);

            if (currentUser == null)
            {
                throw new LoginizationException("User does not exist");
            }

            if (!VerifyPasswordHash(request.Password, currentUser.PasswordHash, currentUser.PasswordSault))
            {
                throw new LoginizationException("Wrong password");
            }

            return CreateAuthToken(currentUser);
        }

        private string CreateAuthToken(User currentUser)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, currentUser.Id),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRolesEnum), currentUser.UserRoles.Select(x => (int)x.Role).Max()))
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _toolbox.GetSectionFromConfig("AppSettings:Token")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSault)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSault = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }
    }
}