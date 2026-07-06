using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Abituria.Data;

[DbContext(typeof(AppDbContext))]
[Migration("202606270001_InitialLocalAccounts")]
public sealed class InitialLocalAccounts : Migration
{
    private static readonly string[] ExerciseProgressIndexColumns = ["ProfileId", "ExerciseId"];

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AppMetadata",
            columns: table => new
            {
                Key = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                Value = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_AppMetadata", row => row.Key));

        migrationBuilder.CreateTable(
            name: "Profiles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                DisplayName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                NormalizedName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                Kind = table.Column<int>(type: "INTEGER", nullable: false),
                PasswordHash = table.Column<byte[]>(type: "BLOB", nullable: true),
                PasswordSalt = table.Column<byte[]>(type: "BLOB", nullable: true),
                PasswordIterations = table.Column<int>(type: "INTEGER", nullable: true),
                RecoveryCodeHash = table.Column<byte[]>(type: "BLOB", nullable: true),
                CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                LastLoginUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Profiles", row => row.Id));

        migrationBuilder.CreateTable(
            name: "ExerciseProgress",
            columns: table => new
            {
                Id = table.Column<long>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                ProfileId = table.Column<Guid>(type: "TEXT", nullable: false),
                ExerciseId = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                CompletedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExerciseProgress", row => row.Id);
                table.ForeignKey("FK_ExerciseProgress_Profiles_ProfileId", row => row.ProfileId, "Profiles", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_Profiles_NormalizedName", "Profiles", "NormalizedName", unique: true);
        migrationBuilder.CreateIndex("IX_ExerciseProgress_ProfileId_ExerciseId", "ExerciseProgress", ExerciseProgressIndexColumns, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("AppMetadata");
        migrationBuilder.DropTable("ExerciseProgress");
        migrationBuilder.DropTable("Profiles");
    }
}
