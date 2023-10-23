using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ContainerType>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<ContainerType>();
        }

        public DbSet<TaskContainer> TaskContainers { get; set; }
    }
}