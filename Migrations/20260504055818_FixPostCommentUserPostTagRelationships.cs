using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogFlow.API.Migrations
{
    /// <inheritdoc />
    public partial class FixPostCommentUserPostTagRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_posts_post_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_post_tags_posts_post_id",
                table: "post_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_post_tags_tags_tag_id",
                table: "post_tags");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Post_Title_MinLength",
                table: "posts");

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "comments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_posts_post_id",
                table: "comments",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_post_tags_posts_post_id",
                table: "post_tags",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_post_tags_tags_tag_id",
                table: "post_tags",
                column: "tag_id",
                principalTable: "tags",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_posts_post_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_post_tags_posts_post_id",
                table: "post_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_post_tags_tags_tag_id",
                table: "post_tags");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "comments");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Post_Title_MinLength",
                table: "posts",
                sql: "length(title) >= 3");

            migrationBuilder.AddForeignKey(
                name: "fk_comments_posts_post_id",
                table: "comments",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_post_tags_posts_post_id",
                table: "post_tags",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_post_tags_tags_tag_id",
                table: "post_tags",
                column: "tag_id",
                principalTable: "tags",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
