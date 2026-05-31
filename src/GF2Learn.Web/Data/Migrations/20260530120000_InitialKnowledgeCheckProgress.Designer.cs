using GF2Learn.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GF2Learn.Web.Data.Migrations;

[DbContext(typeof(Gf2LearnDbContext))]
[Migration("20260530120000_InitialKnowledgeCheckProgress")]
partial class InitialKnowledgeCheckProgress
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
#pragma warning restore 612, 618
    }
}
