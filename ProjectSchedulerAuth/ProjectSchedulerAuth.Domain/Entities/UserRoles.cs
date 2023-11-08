using ProjectSchedulerAuth.Infrastructure.Repositories.Base;

namespace ProjectSchedulerAuth.Domain.Entities
{
    public class UserRoles : AuditEntity<Guid>
    {
        public Guid Id { get; }
        public UserRolesEnum Role { get; }
        public string UserSpecialName { get; }
    }

    public enum UserRolesEnum
    {
        User = 0,
        Admin = 1
    }
}