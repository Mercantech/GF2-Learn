using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GF2Learn.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class HardenLearningGroupAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_learning_group_access_tokens_GroupId",
                table: "learning_group_access_tokens");

            // Keep the newest code if a deployment ever accumulated duplicates before
            // rotation was serialized. This makes the invariant safe to add in-place.
            migrationBuilder.Sql("""
                WITH ranked_codes AS (
                    SELECT
                        "Id",
                        ROW_NUMBER() OVER (
                            PARTITION BY "GroupId"
                            ORDER BY "CreatedAt" DESC, "Id" DESC) AS code_rank
                    FROM learning_group_access_tokens
                    WHERE "Kind" = 1 AND "RevokedAt" IS NULL
                )
                UPDATE learning_group_access_tokens AS access_token
                SET "RevokedAt" = CURRENT_TIMESTAMP
                FROM ranked_codes
                WHERE access_token."Id" = ranked_codes."Id"
                  AND ranked_codes.code_rank > 1;
                """);

            migrationBuilder.CreateIndex(
                name: "UX_learning_group_access_tokens_active_join_code",
                table: "learning_group_access_tokens",
                column: "GroupId",
                unique: true,
                filter: "\"Kind\" = 1 AND \"RevokedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_learning_group_access_tokens_active_join_code",
                table: "learning_group_access_tokens");

            migrationBuilder.CreateIndex(
                name: "IX_learning_group_access_tokens_GroupId",
                table: "learning_group_access_tokens",
                column: "GroupId");
        }
    }
}
