using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Logger> Loggers { get; set; }
        public DbSet<Caffe> Caffes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subgroup> Subgroups { get; set; }
        public DbSet<Article> Articles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Subgroups)
                .WithOne(sg => sg.Group)
                .HasForeignKey(sg => sg.GroupId)
                .OnDelete(DeleteBehavior.Cascade); // kaskadno brisanje za podgrupe

            modelBuilder.Entity<Subgroup>()
                .HasMany(sg => sg.Articles)
                .WithOne(a => a.Subgroup)
                .HasForeignKey(a => a.SubgroupId)
                .OnDelete(DeleteBehavior.Cascade); // kaskadno brisanje za artikle
            
        }
    }
}
