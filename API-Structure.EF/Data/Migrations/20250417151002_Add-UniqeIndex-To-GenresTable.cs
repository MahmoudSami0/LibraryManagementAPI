using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Structure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqeIndexToGenresTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Genres_GenreName",
                table: "Genres",
                column: "GenreName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Genres_GenreName",
                table: "Genres");
        }
    }
}
