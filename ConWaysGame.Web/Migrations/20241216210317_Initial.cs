﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConWaysGame.Web.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TotalGridCeels = table.Column<int>(type: "INTEGER", nullable: false),
                    LiveCells = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false),
                    HasStabilized = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
