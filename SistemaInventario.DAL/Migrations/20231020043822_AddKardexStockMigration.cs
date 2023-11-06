using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaInventario.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddKardexStockMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KardexStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseProductId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastStock = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KardexStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KardexStocks_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KardexStocks_WarehouseProducts_WarehouseProductId",
                        column: x => x.WarehouseProductId,
                        principalTable: "WarehouseProducts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KardexStocks_AppUserId",
                table: "KardexStocks",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KardexStocks_WarehouseProductId",
                table: "KardexStocks",
                column: "WarehouseProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KardexStocks");
        }
    }
}
