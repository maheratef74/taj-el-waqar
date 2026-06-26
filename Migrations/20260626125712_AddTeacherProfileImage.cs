using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElMaherQuranSchool.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Teachers");
        }
    }
}
