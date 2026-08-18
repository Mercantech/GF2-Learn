using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GF2Learn.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthDisplayName",
                table: "app_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthDisplayName",
                table: "app_users");
        }
    }
}
