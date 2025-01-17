using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PinterestApplication.Models;

namespace PinterestApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUser{ get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Board> Board { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Like> Like { get; set; }
        public DbSet<PostBoard> PostBoard { get; set; }
        public DbSet<Badge> Badge { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PostBoard>()
                .HasKey(pb => new { pb.Id, pb.PostId, pb.BoardId });

            modelBuilder.Entity<PostBoard>()
                .HasOne(pb => pb.Post)
                .WithMany(pb => pb.PostBoards)
                .HasForeignKey(pb => pb.PostId);

            modelBuilder.Entity<PostBoard>()
                .HasOne(pb => pb.Board)
                .WithMany(pb => pb.PostBoards)
                .HasForeignKey(pb => pb.BoardId);
        }
    }
}
