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
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<LearnerAdminMetadata> LearnerAdminMetadata => Set<LearnerAdminMetadata>();
    public DbSet<LearningGroup> LearningGroups => Set<LearningGroup>();
    public DbSet<LearningGroupMember> LearningGroupMembers => Set<LearningGroupMember>();
    public DbSet<LearningGroupAccessToken> LearningGroupAccessTokens => Set<LearningGroupAccessToken>();
    public DbSet<PageActivitySession> PageActivitySessions => Set<PageActivitySession>();
    public DbSet<PageActivityDaily> PageActivityDaily => Set<PageActivityDaily>();
    public DbSet<PageActivityCreditGate> PageActivityCreditGates => Set<PageActivityCreditGate>();

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

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("app_users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserSub).HasMaxLength(128).IsRequired();
            entity.Property(e => e.AuthDisplayName).HasMaxLength(256);
            entity.Property(e => e.IsEducator).HasDefaultValue(false);
            entity.Property(e => e.IsSuperAdmin).HasDefaultValue(false);
            entity.HasIndex(e => e.UserSub).IsUnique();
        });

        modelBuilder.Entity<LearnerAdminMetadata>(entity =>
        {
            entity.ToTable("learner_admin_metadata");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Nickname).HasMaxLength(200);
            entity.HasOne(e => e.User)
                .WithOne(u => u.AdminMetadata)
                .HasForeignKey<LearnerAdminMetadata>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LearningGroup>(entity =>
        {
            entity.ToTable("learning_groups");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(160).IsRequired();
            entity.Property(e => e.CreatedBySub).HasMaxLength(128).IsRequired();
        });

        modelBuilder.Entity<LearningGroupMember>(entity =>
        {
            entity.ToTable("learning_group_members");
            entity.HasKey(e => new { e.GroupId, e.UserId });
            entity.Property(e => e.AddedBySub).HasMaxLength(128);
            entity.HasOne(e => e.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LearningGroupAccessToken>(entity =>
        {
            entity.ToTable("learning_group_access_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TokenHash).HasMaxLength(128).IsRequired();
            entity.Property(e => e.DisplaySuffix).HasMaxLength(16);
            entity.Property(e => e.CreatedBySub).HasMaxLength(128).IsRequired();
            entity.Property(e => e.UseCount).IsConcurrencyToken();
            entity.HasIndex(e => e.TokenHash).IsUnique();
            entity.HasIndex(e => e.GroupId)
                .HasDatabaseName("UX_learning_group_access_tokens_active_join_code")
                .IsUnique()
                .HasFilter("\"Kind\" = 1 AND \"RevokedAt\" IS NULL");
            entity.HasOne(e => e.Group)
                .WithMany(g => g.AccessTokens)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PageActivitySession>(entity =>
        {
            entity.ToTable("page_activity_sessions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContentType).HasMaxLength(32).IsRequired();
            entity.Property(e => e.ContentSlug).HasMaxLength(128).IsRequired();
            entity.Property(e => e.Version).IsConcurrencyToken();
            entity.HasIndex(e => e.LastHeartbeatAt);
            entity.HasOne(e => e.User)
                .WithMany(u => u.ActivitySessions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PageActivityDaily>(entity =>
        {
            entity.ToTable("page_activity_daily");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ContentType).HasMaxLength(32).IsRequired();
            entity.Property(e => e.ContentSlug).HasMaxLength(128).IsRequired();
            entity.Property(e => e.Version).IsConcurrencyToken();
            entity.HasIndex(e => new { e.UserId, e.ActivityDate });
            entity.HasIndex(e => new { e.ContentType, e.ContentSlug, e.ActivityDate });
            entity.HasIndex(e => new { e.UserId, e.ContentType, e.ContentSlug, e.ActivityDate }).IsUnique();
            entity.HasOne(e => e.User)
                .WithMany(u => u.DailyActivity)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PageActivityCreditGate>(entity =>
        {
            entity.ToTable("page_activity_credit_gates");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Version).IsConcurrencyToken();
            entity.HasOne(e => e.User)
                .WithOne(u => u.ActivityCreditGate)
                .HasForeignKey<PageActivityCreditGate>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
