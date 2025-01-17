using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinterestApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class photochanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Badge_AspNetUsers_UserId",
                table: "Badge");

            migrationBuilder.DropIndex(
                name: "IX_Badge_UserId",
                table: "Badge");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Badge");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Badge",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Badge",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Badge",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Badge_ApplicationUserId",
                table: "Badge",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Badge_AspNetUsers_ApplicationUserId",
                table: "Badge",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Badge_AspNetUsers_ApplicationUserId",
                table: "Badge");

            migrationBuilder.DropIndex(
                name: "IX_Badge_ApplicationUserId",
                table: "Badge");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Badge");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Badge");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Badge",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Badge",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Badge_UserId",
                table: "Badge",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Badge_AspNetUsers_UserId",
                table: "Badge",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
