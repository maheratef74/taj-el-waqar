using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElMaherQuranSchool.Migrations
{
    /// <inheritdoc />
    public partial class AddHalaqaFieldsLevelLocationCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgeRange",
                table: "Halaqas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassTime",
                table: "Halaqas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "Halaqas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Halaqas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxCapacity",
                table: "Halaqas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeRange",
                table: "Halaqas");

            migrationBuilder.DropColumn(
                name: "ClassTime",
                table: "Halaqas");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Halaqas");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Halaqas");

            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "Halaqas");
        }
    }
}
