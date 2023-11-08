using ProjectSchedulerAuth.Infrastructure.Repositories.Base;
using System.ComponentModel.DataAnnotations;

namespace ProjectSchedulerDomain.Entities
{
    /// <summary>
    /// Таблица пользователей в базе
    /// </summary>
    public class User : DeleteEntity<string>
    {
        [Key]
        public string UserSpecialName { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSault { get; set; }   
    }
}