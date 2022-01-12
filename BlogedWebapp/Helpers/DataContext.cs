using BlogedWebapp.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogedWebapp.Helpers
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<ProfileData> ProfilesData { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<Blog> Blogs { get; set; }

        public virtual DbSet<UsersBlog> UsersBlog { get; set; }

        public virtual DbSet<Post> Posts { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Tag> Tags { get; set; }

        public virtual DbSet<PostsTag> PostsTag { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
