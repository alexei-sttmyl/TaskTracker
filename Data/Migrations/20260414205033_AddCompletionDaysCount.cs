using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletionDaysCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompletionDaysCount",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionDaysCount",
                table: "Habits");
        }
    }
}
