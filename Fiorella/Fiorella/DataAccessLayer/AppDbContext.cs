using Fiorella.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.DataAccessLayer
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SliderImage> SliderImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<AboutServices> AboutServices { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<ExpertsSection> ExpertsSections { get; set; }
        public DbSet<Subscribe> Subscribes{ get; set; }
        public DbSet<BlogSection> BlogSections { get; set; }
        public DbSet<Say> Says { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Bio> Bios { get; set; }
        public DbSet<Instagram> Instagrams { get; set; }

    }
}
