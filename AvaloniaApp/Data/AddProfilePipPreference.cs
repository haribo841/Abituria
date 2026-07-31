using Abituria.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Abituria.Data;

[DbContext(typeof(AppDbContext))]
[Migration("202607310001_AddProfilePipPreference")]
public sealed class AddProfilePipPreference : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<CalculatorPipMode>(
            name: "CalculatorPipMode",
            table: "Profiles",
            type: "INTEGER",
            nullable: false,
            defaultValue: CalculatorPipMode.OwnedWindow);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CalculatorPipMode",
            table: "Profiles");
    }
}
