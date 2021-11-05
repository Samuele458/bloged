using BlogedWebapp.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogedWebapp.Helpers
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        /*
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }*/
    }
}
