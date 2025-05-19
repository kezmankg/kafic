using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class changeOrderPaidTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderPaids_Caffes_CaffeId",
                table: "OrderPaids");

            migrationBuilder.DropIndex(
                name: "IX_OrderPaids_CaffeId",
                table: "OrderPaids");

            migrationBuilder.DropColumn(
                name: "CaffeId",
                table: "OrderPaids");

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "OrderPaids",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "OrderPaids");

            migrationBuilder.AddColumn<int>(
                name: "CaffeId",
                table: "OrderPaids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaids_CaffeId",
                table: "OrderPaids",
                column: "CaffeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderPaids_Caffes_CaffeId",
                table: "OrderPaids",
                column: "CaffeId",
                principalTable: "Caffes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
