using GF2Learn.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Web.Data;

public sealed class Gf2LearnDbContext(DbContextOptions<Gf2LearnDbContext> options) : DbContext(options)
{
    public DbSet<KnowledgeCheckAnswer> KnowledgeCheckAnswers => Set<KnowledgeCheckAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KnowledgeCheckAnswer>(entity =>
        {
            entity.ToTable("knowledge_check_answers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserSub).HasMaxLength(128).IsRequired();
            entity.Property(e => e.ContentSlug).HasMaxLength(128).IsRequired();
            entity.HasIndex(e => new { e.UserSub, e.ContentSlug, e.QuestionIndex }).IsUnique();
        });
    }
}
