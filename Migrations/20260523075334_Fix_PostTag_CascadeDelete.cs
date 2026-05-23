using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogFlow.API.Migrations
{
    /// <inheritdoc />
    public partial class Fix_PostTag_CascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_post_tags_posts_post_id",
                table: "post_tags");

            migrationBuilder.AddForeignKey(
                name: "fk_post_tags_posts_post_id",
                table: "post_tags",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_post_tags_posts_post_id",
                table: "post_tags");

            migrationBuilder.AddForeignKey(
                name: "fk_post_tags_posts_post_id",
                table: "post_tags",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
