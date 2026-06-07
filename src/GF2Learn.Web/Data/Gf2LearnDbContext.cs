using GF2Learn.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Web.Data;

public sealed class Gf2LearnDbContext(DbContextOptions<Gf2LearnDbContext> options) : DbContext(options)
{
    public DbSet<KnowledgeCheckAnswer> KnowledgeCheckAnswers => Set<KnowledgeCheckAnswer>();
    public DbSet<ExerciseAnswer> ExerciseAnswers => Set<ExerciseAnswer>();
    public DbSet<ExercisePartVerification> ExercisePartVerifications => Set<ExercisePartVerification>();
    public DbSet<PlaygroundProject> PlaygroundProjects => Set<PlaygroundProject>();
    public DbSet<PlaygroundFile> PlaygroundFiles => Set<PlaygroundFile>();

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

        modelBuilder.Entity<ExerciseAnswer>(entity =>
        {
            entity.ToTable("exercise_answers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserSub).HasMaxLength(128).IsRequired();
            entity.Property(e => e.ContentSlug).HasMaxLength(128).IsRequired();
            entity.Property(e => e.AnswerText).HasMaxLength(16_000);
            entity.HasIndex(e => new { e.UserSub, e.ContentSlug, e.PartIndex });
        });

        modelBuilder.Entity<ExercisePartVerification>(entity =>
        {
            entity.ToTable("exercise_part_verifications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserSub).HasMaxLength(128).IsRequired();
            entity.Property(e => e.ContentSlug).HasMaxLength(128).IsRequired();
            entity.HasIndex(e => new { e.UserSub, e.ContentSlug, e.PartIndex }).IsUnique();
        });

        modelBuilder.Entity<PlaygroundProject>(entity =>
        {
            entity.ToTable("playground_projects");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserSub).HasMaxLength(128).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(128).IsRequired();
            entity.HasIndex(e => e.UserSub);
        });

        modelBuilder.Entity<PlaygroundFile>(entity =>
        {
            entity.ToTable("playground_files");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).HasMaxLength(128).IsRequired();
            entity.Property(e => e.Content).HasMaxLength(64_000);
            entity.HasIndex(e => new { e.ProjectId, e.FileName }).IsUnique();
            entity.HasOne(e => e.Project)
                .WithMany(p => p.Files)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
