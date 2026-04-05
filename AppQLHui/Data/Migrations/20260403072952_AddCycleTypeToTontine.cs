using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppQLHui.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCycleTypeToTontine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CycleType",
                table: "Tontines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CycleType",
                table: "Tontines");
        }
    }
}
