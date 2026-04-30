using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogFlow.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "display_name",
                table: "categories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "display_name",
                table: "categories");
        }
    }
}
