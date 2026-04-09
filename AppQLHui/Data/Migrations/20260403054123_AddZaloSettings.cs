using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppQLHui.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddZaloSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ZaloSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendDelayMs = table.Column<int>(type: "int", nullable: false),
                    PasteX = table.Column<int>(type: "int", nullable: false),
                    PasteY = table.Column<int>(type: "int", nullable: false),
                    PasteShortcut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SendShortcut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsSingleSendOnly = table.Column<bool>(type: "bit", nullable: false),
                    AutoCopyBill = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZaloSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ZaloSettings");
        }
    }
}
