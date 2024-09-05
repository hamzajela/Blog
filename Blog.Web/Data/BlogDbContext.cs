using Blog.Web.Models.Domain;
using Blog.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }


        public DbSet<BlogPost> Blog_Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<BlogPostLike> BlogPostLike { get; set; }
        
        public DbSet<BlogPostComment> BlogPostComment { get; set;}
    }
}
