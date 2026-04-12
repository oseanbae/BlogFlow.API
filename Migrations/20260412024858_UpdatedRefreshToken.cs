using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogFlow.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReplacedByTokenId",
                table: "RefreshTokens",
                newName: "ReplacedByToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReplacedByToken",
                table: "RefreshTokens",
                newName: "ReplacedByTokenId");
        }
    }
}
