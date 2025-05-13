using API_Structure.Core.Consts;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Structure.EF.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: ["Id", "RoleName"],
                values: [Guid.NewGuid().ToString(), Roles.Admin]
                );
            migrationBuilder.InsertData(
                table: "Roles",
                columns: ["Id", "RoleName"],
                values: [Guid.NewGuid().ToString(), Roles.Manager]
                );
            migrationBuilder.InsertData(
                table: "Roles",
                columns: ["Id", "RoleName"],
                values: [Guid.NewGuid().ToString(), Roles.User]
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"Delete From Roles");
        }
    }
}
