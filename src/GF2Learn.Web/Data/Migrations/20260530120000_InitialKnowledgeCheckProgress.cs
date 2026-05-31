using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GF2Learn.Web.Data.Migrations;

public partial class InitialKnowledgeCheckProgress : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "knowledge_check_answers",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserSub = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                ContentSlug = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                QuestionIndex = table.Column<int>(type: "integer", nullable: false),
                SelectedIndex = table.Column<int>(type: "integer", nullable: false),
                IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                AnsweredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_knowledge_check_answers", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_knowledge_check_answers_UserSub_ContentSlug_QuestionIndex",
            table: "knowledge_check_answers",
            columns: new[] { "UserSub", "ContentSlug", "QuestionIndex" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "knowledge_check_answers");
    }
}
