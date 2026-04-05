using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppQLHui.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "ZaloSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Tontines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ZaloSettings_OwnerId",
                table: "ZaloSettings",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tontines_OwnerId",
                table: "Tontines",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_OwnerId",
                table: "Players",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Users_OwnerId",
                table: "Players",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tontines_Users_OwnerId",
                table: "Tontines",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ZaloSettings_Users_OwnerId",
                table: "ZaloSettings",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Users_OwnerId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Tontines_Users_OwnerId",
                table: "Tontines");

            migrationBuilder.DropForeignKey(
                name: "FK_ZaloSettings_Users_OwnerId",
                table: "ZaloSettings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ZaloSettings_OwnerId",
                table: "ZaloSettings");

            migrationBuilder.DropIndex(
                name: "IX_Tontines_OwnerId",
                table: "Tontines");

            migrationBuilder.DropIndex(
                name: "IX_Players_OwnerId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ZaloSettings");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Tontines");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Players");
        }
    }
}
