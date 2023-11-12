using ProjectSchedulerAuth.Domain.Entities;
using ProjectSchedulerAuth.Infrastructure.Repositories.Base;

namespace ProjectSchedulerDomain.Entities
{
    /// <summary>
    /// Таблица пользователей в базе
    /// </summary>
    public class User : EntityBase<string>
    {
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSault { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public User(string username, string id, byte[] passwordHash, byte[] passwordSault)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            PasswordSault = passwordSault;
            UserRoles.Add(new UserRole(UserRolesEnum.User, Id));
        }

        private User(string id) : base()
        {
            Id = id;
        }
    }
}