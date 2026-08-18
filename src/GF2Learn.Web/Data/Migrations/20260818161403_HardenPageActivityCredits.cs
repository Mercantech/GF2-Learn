using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GF2Learn.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class HardenPageActivityCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "page_activity_sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "VisitCredited",
                table: "page_activity_sessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Every pre-migration session already contributed its visit when it was
            // created. Mark those rows so the next heartbeat cannot count it twice.
            migrationBuilder.Sql(
                "UPDATE page_activity_sessions SET \"VisitCredited\" = TRUE");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "page_activity_daily",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "page_activity_credit_gates",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AvailableSeconds = table.Column<int>(type: "integer", nullable: false),
                    LastRefillAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_page_activity_credit_gates", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_page_activity_credit_gates_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "page_activity_credit_gates");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "page_activity_sessions");

            migrationBuilder.DropColumn(
                name: "VisitCredited",
                table: "page_activity_sessions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "page_activity_daily");

        }
    }
}
