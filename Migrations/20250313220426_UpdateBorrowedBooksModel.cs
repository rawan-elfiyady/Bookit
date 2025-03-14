using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookit.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBorrowedBooksModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowedAt",
                table: "BorrowedBooks");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "BorrowedBooks",
                newName: "BorrowedDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "BorrowDate",
                table: "BorrowedBooks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "BorrowedBooks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "BorrowedBooks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "BorrowedBooks",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowDate",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "BorrowedBooks");

            migrationBuilder.RenameColumn(
                name: "BorrowedDate",
                table: "BorrowedBooks",
                newName: "DueDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "BorrowedAt",
                table: "BorrowedBooks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
