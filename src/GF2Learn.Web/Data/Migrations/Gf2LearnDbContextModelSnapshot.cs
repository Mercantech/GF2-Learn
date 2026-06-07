using GF2Learn.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GF2Learn.Web.Data.Migrations;

[DbContext(typeof(Gf2LearnDbContext))]
partial class Gf2LearnDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.5")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("GF2Learn.Web.Models.KnowledgeCheckAnswer", b =>
            {
                b.Property<long>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("bigint");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                b.Property<DateTimeOffset>("AnsweredAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("ContentSlug")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnType("character varying(128)");

                b.Property<bool>("IsCorrect")
                    .HasColumnType("boolean");

                b.Property<int>("QuestionIndex")
                    .HasColumnType("integer");

                b.Property<int>("SelectedIndex")
                    .HasColumnType("integer");

                b.Property<string>("UserSub")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnType("character varying(128)");

                b.HasKey("Id");

                b.HasIndex("UserSub", "ContentSlug", "QuestionIndex")
                    .IsUnique();

                b.ToTable("knowledge_check_answers", (string)null);
            });

        modelBuilder.Entity("GF2Learn.Web.Models.ExerciseAnswer", b =>
            {
                b.Property<long>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("bigint");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                b.Property<string>("AnswerText")
                    .HasMaxLength(16000)
                    .HasColumnType("character varying(16000)");

                b.Property<DateTimeOffset>("CompletedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("ContentSlug")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnType("character varying(128)");

                b.Property<int>("PartIndex")
                    .HasColumnType("integer");

                b.Property<string>("UserSub")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnType("character varying(128)");

                b.HasKey("Id");

                b.HasIndex("UserSub", "ContentSlug", "PartIndex");

                b.ToTable("exercise_answers", (string)null);
            });

        modelBuilder.Entity("GF2Learn.Web.Models.ExercisePartVerification", b =>
            {
                b.Property<long>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("bigint");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                b.Property<string>("ContentSlug")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnType("character varying(128)");

                b.Property<bool>("IsSolved")
                    .HasColumnType("boolean");

                b.Property<int>("PartIndex")
                    .HasColumnType("integer");

                b.Property<string>("UserSub")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnType("character varying(128)");

                b.Property<DateTimeOffset>("VerifiedAt")
                    .HasColumnType("timestamp with time zone");

                b.HasKey("Id");

                b.HasIndex("UserSub", "ContentSlug", "PartIndex")
                    .IsUnique();

                b.ToTable("exercise_part_verifications", (string)null);
            });

        modelBuilder.Entity("GF2Learn.Web.Models.PlaygroundFile", b =>
            {
                b.Property<long>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("bigint");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                b.Property<string>("Content")
                    .HasMaxLength(64000)
                    .HasColumnType("character varying(64000)");

                b.Property<string>("FileName")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnType("character varying(128)");

                b.Property<Guid>("ProjectId")
                    .HasColumnType("uuid");

                b.Property<int>("SortOrder")
                    .HasColumnType("integer");

                b.Property<DateTimeOffset>("UpdatedAt")
                    .HasColumnType("timestamp with time zone");

                b.HasKey("Id");

                b.HasIndex("ProjectId", "FileName")
                    .IsUnique();

                b.ToTable("playground_files", (string)null);
            });

        modelBuilder.Entity("GF2Learn.Web.Models.PlaygroundProject", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<DateTimeOffset>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnType("character varying(128)");

                b.Property<DateTimeOffset>("UpdatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("UserSub")
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnType("character varying(128)");

                b.HasKey("Id");

                b.HasIndex("UserSub");

                b.ToTable("playground_projects", (string)null);
            });

        modelBuilder.Entity("GF2Learn.Web.Models.PlaygroundFile", b =>
            {
                b.HasOne("GF2Learn.Web.Models.PlaygroundProject", "Project")
                    .WithMany("Files")
                    .HasForeignKey("ProjectId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Project");
            });

        modelBuilder.Entity("GF2Learn.Web.Models.PlaygroundProject", b =>
            {
                b.Navigation("Files");
            });
#pragma warning restore 612, 618
    }
}
