using Microsoft.EntityFrameworkCore;
using TestAppDev.Models;

namespace TestAppDev.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Node> Nodes { get; set; }
    public DbSet<ExceptionJournal> ExceptionJournals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Node>(entity =>
        {
            entity.HasKey(n => n.Id);

            entity.Property(n => n.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(n => n.TreeId)
                .IsRequired();

            entity.Property(n => n.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            entity.HasOne(n => n.Parent)
                .WithMany(n => n.Children)
                .HasForeignKey(n => n.ParentId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            entity.HasIndex(n => n.TreeId);
            entity.HasIndex(n => n.ParentId);
            entity.HasIndex(n => n.Name); 

            entity.Property(n => n.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            entity.HasIndex(n => new { n.TreeId, n.Name, n.ParentId })
                .IsUnique()
                .HasFilter("\"ParentId\" IS NOT NULL"); 
        });

        modelBuilder.Entity<ExceptionJournal>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.EventId)
                .IsRequired()
                .HasMaxLength(36);

            entity.Property(e => e.ExceptionType)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.StackTrace)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.ExceptionMessage)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.QueryParameters)
                .HasColumnType("text");

            entity.Property(e => e.BodyParameters)
                .HasColumnType("text");

            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.ExceptionType);
        });
    }
}
