using ProjectSchedulerAuth.Infrastructure.Repositories.Base;
using ProjectSchedulerDomain.Entities;

namespace ProjectSchedulerAuth.Domain.Entities
{
    public class UserRole : EntityBase<Guid>
    {
        public UserRolesEnum Role { get; set; }
        public string UserSpecialName { get; set; }
        public User? User { get; set; }

        public UserRole(UserRolesEnum role, string specialname)
        {
            Id = Guid.NewGuid();
            Role = role;
            UserSpecialName = specialname;
        }

        private UserRole(Guid id) : base()
        {
            Id = id;
        }
    }

    public enum UserRolesEnum
    {
        User = 0,
        Admin = 1
    }
}