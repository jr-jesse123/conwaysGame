using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConWaysGame.Web.Migrations
{
    /// <inheritdoc />
    public partial class fixColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LiveCeels",
                table: "Games",
                newName: "LiveCells");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LiveCells",
                table: "Games",
                newName: "LiveCeels");
        }
    }
}
