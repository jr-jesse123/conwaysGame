using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConWaysGame.Web.Migrations
{
    /// <inheritdoc />
    public partial class LiveCeelsProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LiveCeels",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LiveCeels",
                table: "Games");
        }
    }
}
