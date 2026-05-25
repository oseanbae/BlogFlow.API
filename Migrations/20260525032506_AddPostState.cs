using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogFlow.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPostState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_posts_author_id_deleted_at",
                table: "posts");

            migrationBuilder.AddColumn<int>(
                name: "state",
                table: "posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_posts_author_id_state_deleted_at",
                table: "posts",
                columns: new[] { "author_id", "state", "deleted_at" });

            migrationBuilder.CreateIndex(
                name: "ix_posts_state",
                table: "posts",
                column: "state");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_posts_author_id_state_deleted_at",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "ix_posts_state",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "state",
                table: "posts");

            migrationBuilder.CreateIndex(
                name: "ix_posts_author_id_deleted_at",
                table: "posts",
                columns: new[] { "author_id", "deleted_at" });
        }
    }
}
