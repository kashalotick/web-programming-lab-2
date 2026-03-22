using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace web_programming_lab_2.Migrations
{
    /// <inheritdoc />
    public partial class sdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_positive_capacity",
                table: "rooms");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "rooms",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "rooms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "rooms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "grand_total",
                table: "reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "guest_count",
                table: "reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "guest_id",
                table: "reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "guests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(255)", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guests", x => x.id);
                    table.CheckConstraint("ck_email", "email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$'");
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_positive_capacity",
                table: "rooms",
                sql: "capacity > 0");

            migrationBuilder.CreateIndex(
                name: "ix_reservations_guest_id",
                table: "reservations",
                column: "guest_id");

            migrationBuilder.AddCheckConstraint(
                name: "ck_positive_grand_total",
                table: "reservations",
                sql: "grand_total >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_positive_guest_count",
                table: "reservations",
                sql: "guest_count > 0");

            migrationBuilder.CreateIndex(
                name: "ix_guests_email",
                table: "guests",
                column: "email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_reservations_guests_guest_id",
                table: "reservations",
                column: "guest_id",
                principalTable: "guests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservations_guests_guest_id",
                table: "reservations");

            migrationBuilder.DropTable(
                name: "guests");

            migrationBuilder.DropCheckConstraint(
                name: "ck_positive_capacity",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_reservations_guest_id",
                table: "reservations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_positive_grand_total",
                table: "reservations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_positive_guest_count",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "description",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "grand_total",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "guest_count",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "guest_id",
                table: "reservations");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "rooms",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_positive_capacity",
                table: "rooms",
                sql: "capacity >= 0");
        }
    }
}
