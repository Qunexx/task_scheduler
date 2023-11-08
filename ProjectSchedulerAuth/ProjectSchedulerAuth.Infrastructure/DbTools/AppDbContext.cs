using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProjectSchedulerAuth.Domain.Entities;
using ProjectSchedulerDomain.Entities;

namespace ProjectSchedulerAuth.Infrastructure.DbTools
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<UserRolesEnum>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<UserRolesEnum>();
        }

        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
    }
}