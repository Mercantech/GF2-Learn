using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GF2Learn.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminActivityIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_page_activity_sessions_LastHeartbeatAt",
                table: "page_activity_sessions",
                column: "LastHeartbeatAt");

            migrationBuilder.CreateIndex(
                name: "IX_page_activity_daily_ContentType_ContentSlug_ActivityDate",
                table: "page_activity_daily",
                columns: new[] { "ContentType", "ContentSlug", "ActivityDate" });

            migrationBuilder.CreateIndex(
                name: "IX_page_activity_daily_UserId_ActivityDate",
                table: "page_activity_daily",
                columns: new[] { "UserId", "ActivityDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_page_activity_sessions_LastHeartbeatAt",
                table: "page_activity_sessions");

            migrationBuilder.DropIndex(
                name: "IX_page_activity_daily_ContentType_ContentSlug_ActivityDate",
                table: "page_activity_daily");

            migrationBuilder.DropIndex(
                name: "IX_page_activity_daily_UserId_ActivityDate",
                table: "page_activity_daily");
        }
    }
}
