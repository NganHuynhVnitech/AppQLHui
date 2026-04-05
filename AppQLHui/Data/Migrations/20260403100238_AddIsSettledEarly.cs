using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppQLHui.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSettledEarly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSettledEarly",
                table: "TontineShares",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSettledEarly",
                table: "TontineShares");
        }
    }
}
