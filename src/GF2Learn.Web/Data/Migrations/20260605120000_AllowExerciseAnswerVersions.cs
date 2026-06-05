using GF2Learn.Web.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GF2Learn.Web.Data.Migrations;

[DbContext(typeof(Gf2LearnDbContext))]
[Migration("20260605120000_AllowExerciseAnswerVersions")]
public partial class AllowExerciseAnswerVersions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_exercise_answers_UserSub_ContentSlug_PartIndex",
            table: "exercise_answers");

        migrationBuilder.CreateIndex(
            name: "IX_exercise_answers_UserSub_ContentSlug_PartIndex",
            table: "exercise_answers",
            columns: new[] { "UserSub", "ContentSlug", "PartIndex" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_exercise_answers_UserSub_ContentSlug_PartIndex",
            table: "exercise_answers");

        migrationBuilder.CreateIndex(
            name: "IX_exercise_answers_UserSub_ContentSlug_PartIndex",
            table: "exercise_answers",
            columns: new[] { "UserSub", "ContentSlug", "PartIndex" },
            unique: true);
    }
}
