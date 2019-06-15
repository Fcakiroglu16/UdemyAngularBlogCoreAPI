using Microsoft.EntityFrameworkCore;

namespace UdemyAngularBlogCore.API.Models
{
    public partial class UdemyAngularBlogDBContext : DbContext
    {
        public UdemyAngularBlogDBContext()
        {
        }

        public UdemyAngularBlogDBContext(DbContextOptions<UdemyAngularBlogDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Article>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.ContentMain)
                    .IsRequired()
                    .HasColumnName("content_main");

                entity.Property(e => e.ContentSummary)
                    .IsRequired()
                    .HasColumnName("content_summary")
                    .HasMaxLength(500);

                entity.Property(e => e.Picture)
                    .HasColumnName("picture")
                    .HasMaxLength(300);

                entity.Property(e => e.PublishDate)
                    .HasColumnName("publish_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(500);

                entity.Property(e => e.ViewCount).HasColumnName("viewCount");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Article)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Article_Category");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");

                entity.Property(e => e.ContentMain)
                    .IsRequired()
                    .HasColumnName("content_main");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.PublishDate)
                    .HasColumnName("publish_date")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_Comment_Article");
            });
        }
    }
}