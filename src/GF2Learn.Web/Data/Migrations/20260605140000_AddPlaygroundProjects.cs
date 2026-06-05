using System;
using GF2Learn.Web.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GF2Learn.Web.Data.Migrations;

[DbContext(typeof(Gf2LearnDbContext))]
[Migration("20260605140000_AddPlaygroundProjects")]
public partial class AddPlaygroundProjects : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "playground_projects",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserSub = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_playground_projects", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "playground_files",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                FileName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Content = table.Column<string>(type: "character varying(64000)", maxLength: 64000, nullable: true),
                SortOrder = table.Column<int>(type: "integer", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_playground_files", x => x.Id);
                table.ForeignKey(
                    name: "FK_playground_files_playground_projects_ProjectId",
                    column: x => x.ProjectId,
                    principalTable: "playground_projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_playground_projects_UserSub",
            table: "playground_projects",
            column: "UserSub");

        migrationBuilder.CreateIndex(
            name: "IX_playground_files_ProjectId_FileName",
            table: "playground_files",
            columns: new[] { "ProjectId", "FileName" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "playground_files");
        migrationBuilder.DropTable(name: "playground_projects");
    }
}
