using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GF2Learn.Web.Data.Migrations;

public partial class AddExercisePartVerifications : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "exercise_part_verifications",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserSub = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                ContentSlug = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                PartIndex = table.Column<int>(type: "integer", nullable: false),
                IsSolved = table.Column<bool>(type: "boolean", nullable: false),
                VerifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_exercise_part_verifications", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_exercise_part_verifications_UserSub_ContentSlug_PartIndex",
            table: "exercise_part_verifications",
            columns: new[] { "UserSub", "ContentSlug", "PartIndex" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "exercise_part_verifications");
    }
}
