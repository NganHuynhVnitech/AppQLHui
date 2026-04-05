using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppQLHui.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CycleType",
                table: "Tontines",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CycleNotes",
                table: "Tontines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Players",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CycleNotes",
                table: "Tontines");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Players");

            migrationBuilder.AlterColumn<string>(
                name: "CycleType",
                table: "Tontines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
