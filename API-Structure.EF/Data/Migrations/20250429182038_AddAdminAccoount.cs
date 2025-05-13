using API_Structure.Core.Consts;
using API_Structure.Core.Helpers;
using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Structure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAccoount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
            table: "Users",
            columns: ["Id", "FirstName", "LastName",
                "Email", "UserName", "Password",
                "Phone", "Photo", "IsDeleted",
                "IsEmailConfirmed", "EmailConfirmationToken", "EmailConfirmationTokenExpiration"],
            values: [Guid.NewGuid().ToString(), "admin","admin",
                "admin@gmail.com","Admin12",PasswordHelper.HashPassword("Admin@123"),
                null,null,false,
                true,null,null]
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete From Users Where Email = admin@gmail.com");
        }
    }
}
