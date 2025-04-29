using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookit.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernameToBorrowedBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "BorrowedBooks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "BorrowedBooks");
        }
    }
}
