using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestAppDev.Migrations
{
    /// <inheritdoc />
    public partial class AddExceptionMessageToExceptionJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExceptionMessage",
                table: "ExceptionJournals",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExceptionMessage",
                table: "ExceptionJournals");
        }
    }
}
