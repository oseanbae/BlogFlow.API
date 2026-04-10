using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogFlow.API.Migrations
{
    /// <inheritdoc />
    public partial class UserRoleEnumRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE ""Users""
            ALTER COLUMN ""Role"" TYPE integer
            USING CASE
                WHEN ""Role"" = 'Admin' THEN 1
                WHEN ""Role"" = 'Author' THEN 2
                WHEN ""Role"" = 'Reader' THEN 3
                ELSE 3
            END;
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE ""Users""
            ALTER COLUMN ""Role"" TYPE varchar(20)
            USING CASE
                WHEN ""Role"" = 1 THEN 'Admin'
                WHEN ""Role"" = 2 THEN 'Author'
                WHEN ""Role"" = 3 THEN 'Reader'
                ELSE 'Reader'
            END;
        ");
        }
    }
}
