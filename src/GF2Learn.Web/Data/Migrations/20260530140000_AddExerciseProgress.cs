using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GF2Learn.Web.Data.Migrations;

public partial class AddExerciseProgress : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "exercise_answers",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserSub = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                ContentSlug = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                PartIndex = table.Column<int>(type: "integer", nullable: false),
                AnswerText = table.Column<string>(type: "character varying(16000)", maxLength: 16000, nullable: true),
                CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_exercise_answers", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_exercise_answers_UserSub_ContentSlug_PartIndex",
            table: "exercise_answers",
            columns: new[] { "UserSub", "ContentSlug", "PartIndex" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "exercise_answers");
    }
}
