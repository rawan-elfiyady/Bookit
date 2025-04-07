using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookit.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRequestedToBeReturnedToBorrowedBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRequestedToBeReturned",
                table: "BorrowedBooks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRequestedToBeReturned",
                table: "BorrowedBooks");
        }
    }
}
