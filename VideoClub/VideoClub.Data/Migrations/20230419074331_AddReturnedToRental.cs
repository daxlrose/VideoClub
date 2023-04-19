using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoClub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReturnedToRental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReturnDate",
                table: "Rentals",
                newName: "DueDate");

            migrationBuilder.AddColumn<bool>(
                name: "Returned",
                table: "Rentals",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Returned",
                table: "Rentals");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Rentals",
                newName: "ReturnDate");
        }
    }
}
