using BlogApi.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ============================
        // DbSets
        // ============================

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // ============================
            // User Primary Key
            // ============================

            modelBuilder.Entity<User>()
                .HasKey(u => u.User_id);


            // ============================
            // Post Primary Key
            // ============================

            modelBuilder.Entity<Post>()
                .HasKey(p => p.Id);


            // ============================
            // Comment Primary Key
            // ============================

            modelBuilder.Entity<Comment>()
                .HasKey(c => c.Id);


            // ============================
            // Category Primary Key
            // ============================

            modelBuilder.Entity<Category>()
                .HasKey(c => c.Id);


            // ============================
            // Tag Primary Key
            // ============================

            modelBuilder.Entity<Tag>()
                .HasKey(t => t.Id);


            // ============================
            // PostTag Composite Primary Key
            // ============================

            modelBuilder.Entity<PostTag>()
                .HasKey(pt => new
                {
                    pt.PostId,
                    pt.TagId
                });


            // ============================
            // User -> Posts
            // One User has Many Posts
            // ============================

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================
            // Category -> Posts
            // One Category has Many Posts
            // ============================

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================
            // Post -> Comments
            // One Post has Many Comments
            // ============================

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);


            // ============================
            // User -> Comments
            // One User has Many Comments
            // ============================

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================
            // PostTag -> Post
            // ============================

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId)
                .OnDelete(DeleteBehavior.Cascade);


            // ============================
            // PostTag -> Tag
            // ============================

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade);


            // ============================
            // User Email Unique
            // ============================

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            // ============================
            // User Username Unique
            // ============================

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}