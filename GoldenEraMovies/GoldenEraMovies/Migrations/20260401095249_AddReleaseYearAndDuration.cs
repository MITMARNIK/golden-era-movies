using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoldenEraMovies.Migrations
{
    /// <inheritdoc />
    public partial class AddReleaseYearAndDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseYear",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "ReleaseYear",
                table: "Movies");
        }
    }
}
