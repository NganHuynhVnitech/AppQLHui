using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppQLHui.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            // Fix for existing nulls before making NOT NULL
            migrationBuilder.Sql("UPDATE Players SET OwnerId = 1 WHERE OwnerId IS NULL OR OwnerId = 0");
            migrationBuilder.Sql("UPDATE Tontines SET OwnerId = 1 WHERE OwnerId IS NULL OR OwnerId = 0");
            migrationBuilder.Sql("UPDATE ZaloSettings SET OwnerId = 1 WHERE OwnerId IS NULL OR OwnerId = 0");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "ZaloSettings",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Tontines",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Users_OwnerId",
                table: "Players",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tontines_Users_OwnerId",
                table: "Tontines",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ZaloSettings_Users_OwnerId",
                table: "ZaloSettings",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "ZaloSettings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Tontines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Players",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}
