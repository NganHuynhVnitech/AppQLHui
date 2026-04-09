using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppQLHui.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ZaloName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tontines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    BaseAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TotalShares = table.Column<int>(type: "int", nullable: false),
                    CommissionFee = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    FeeType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalFeesCollected = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tontines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Draws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TontineId = table.Column<int>(type: "int", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    DrawDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WinningShareId = table.Column<int>(type: "int", nullable: true),
                    BidAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    CollectedFee = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Draws", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Draws_Tontines_TontineId",
                        column: x => x.TontineId,
                        principalTable: "Tontines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TontineShares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TontineId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WonDrawId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TontineShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TontineShares_Draws_WonDrawId",
                        column: x => x.WonDrawId,
                        principalTable: "Draws",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TontineShares_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TontineShares_Tontines_TontineId",
                        column: x => x.TontineId,
                        principalTable: "Tontines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DrawId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    AmountPay = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    AmountReceive = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    IsSettled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Draws_DrawId",
                        column: x => x.DrawId,
                        principalTable: "Draws",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Draws_TontineId",
                table: "Draws",
                column: "TontineId");

            migrationBuilder.CreateIndex(
                name: "IX_Draws_WinningShareId",
                table: "Draws",
                column: "WinningShareId");

            migrationBuilder.CreateIndex(
                name: "IX_TontineShares_PlayerId",
                table: "TontineShares",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TontineShares_TontineId",
                table: "TontineShares",
                column: "TontineId");

            migrationBuilder.CreateIndex(
                name: "IX_TontineShares_WonDrawId",
                table: "TontineShares",
                column: "WonDrawId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DrawId",
                table: "Transactions",
                column: "DrawId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PlayerId",
                table: "Transactions",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Draws_TontineShares_WinningShareId",
                table: "Draws",
                column: "WinningShareId",
                principalTable: "TontineShares",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Draws_TontineShares_WinningShareId",
                table: "Draws");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TontineShares");

            migrationBuilder.DropTable(
                name: "Draws");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Tontines");
        }
    }
}
