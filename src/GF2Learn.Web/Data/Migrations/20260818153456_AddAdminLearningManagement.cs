using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GF2Learn.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminLearningManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserSub = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsEducator = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsSuperAdmin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FirstSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastActivityAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_users", x => x.Id);
                });

            // Existing records prove that the user had an authenticated session at the
            // observed timestamp. They do not expose a separate historic login timestamp,
            // so the latest known activity is the most conservative useful login anchor.
            migrationBuilder.Sql(
                """
                CREATE TEMP TABLE gf2_admin_observed_activity (
                    "UserSub" character varying(128) NOT NULL,
                    observed_at timestamp with time zone NOT NULL
                ) ON COMMIT DROP;

                INSERT INTO gf2_admin_observed_activity ("UserSub", observed_at)
                SELECT "UserSub", "AnsweredAt"
                FROM knowledge_check_answers
                UNION ALL
                SELECT "UserSub", "CompletedAt"
                FROM exercise_answers
                UNION ALL
                SELECT "UserSub", "CreatedAt"
                FROM playground_projects
                UNION ALL
                SELECT "UserSub", "UpdatedAt"
                FROM playground_projects;

                -- This table exists in the current model, but an older migration was not
                -- discoverable by EF in every installation. Include it whenever present
                -- without making this data-preserving migration fail on older databases.
                DO $backfill$
                BEGIN
                    IF to_regclass('exercise_part_verifications') IS NOT NULL THEN
                        EXECUTE '
                            INSERT INTO gf2_admin_observed_activity ("UserSub", observed_at)
                            SELECT "UserSub", "VerifiedAt"
                            FROM exercise_part_verifications';
                    END IF;
                END
                $backfill$;

                WITH summarized_activity AS (
                    SELECT
                        "UserSub",
                        MIN(observed_at) AS first_seen_at,
                        MAX(observed_at) AS last_seen_at
                    FROM gf2_admin_observed_activity
                    WHERE BTRIM("UserSub") <> ''
                    GROUP BY "UserSub"
                )
                INSERT INTO app_users (
                    "Id",
                    "UserSub",
                    "IsEducator",
                    "IsSuperAdmin",
                    "FirstSeenAt",
                    "LastLoginAt",
                    "LastActivityAt")
                SELECT
                    gen_random_uuid(),
                    "UserSub",
                    FALSE,
                    FALSE,
                    first_seen_at,
                    last_seen_at,
                    last_seen_at
                FROM summarized_activity;

                DROP TABLE gf2_admin_observed_activity;
                """);

            migrationBuilder.CreateTable(
                name: "learning_groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBySub = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learning_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "learner_admin_metadata",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nickname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learner_admin_metadata", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_learner_admin_metadata_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "page_activity_daily",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ContentSlug = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ActivityDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ActiveSeconds = table.Column<int>(type: "integer", nullable: false),
                    VisitCount = table.Column<int>(type: "integer", nullable: false),
                    LastSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_page_activity_daily", x => x.Id);
                    table.ForeignKey(
                        name: "FK_page_activity_daily_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "page_activity_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ContentSlug = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ReportedActiveSeconds = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastHeartbeatAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_page_activity_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_page_activity_sessions_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learning_group_access_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplaySuffix = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    MaxUses = table.Column<int>(type: "integer", nullable: true),
                    UseCount = table.Column<int>(type: "integer", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBySub = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learning_group_access_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_learning_group_access_tokens_learning_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "learning_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learning_group_members",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    AddedBySub = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learning_group_members", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_learning_group_members_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_learning_group_members_learning_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "learning_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_app_users_UserSub",
                table: "app_users",
                column: "UserSub",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_learning_group_access_tokens_GroupId",
                table: "learning_group_access_tokens",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_learning_group_access_tokens_TokenHash",
                table: "learning_group_access_tokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_learning_group_members_UserId",
                table: "learning_group_members",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_page_activity_daily_UserId_ContentType_ContentSlug_Activity~",
                table: "page_activity_daily",
                columns: new[] { "UserId", "ContentType", "ContentSlug", "ActivityDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_page_activity_sessions_UserId",
                table: "page_activity_sessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "learner_admin_metadata");

            migrationBuilder.DropTable(
                name: "learning_group_access_tokens");

            migrationBuilder.DropTable(
                name: "learning_group_members");

            migrationBuilder.DropTable(
                name: "page_activity_daily");

            migrationBuilder.DropTable(
                name: "page_activity_sessions");

            migrationBuilder.DropTable(
                name: "learning_groups");

            migrationBuilder.DropTable(
                name: "app_users");
        }
    }
}
