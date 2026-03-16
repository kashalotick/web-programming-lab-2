using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_programming_lab_2.Migrations
{
    /// <inheritdoc />
    public partial class roomfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Rooms");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "Rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
