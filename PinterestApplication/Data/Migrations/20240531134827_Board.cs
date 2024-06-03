using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinterestApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class Board : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Board");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Board",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Board",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
