using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class addBillTOOrderPaidTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillId",
                table: "OrderPaids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BillId1",
                table: "OrderPaids",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaids_BillId",
                table: "OrderPaids",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaids_BillId1",
                table: "OrderPaids",
                column: "BillId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderPaids_Bills_BillId",
                table: "OrderPaids",
                column: "BillId",
                principalTable: "Bills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderPaids_Bills_BillId1",
                table: "OrderPaids",
                column: "BillId1",
                principalTable: "Bills",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderPaids_Bills_BillId",
                table: "OrderPaids");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderPaids_Bills_BillId1",
                table: "OrderPaids");

            migrationBuilder.DropIndex(
                name: "IX_OrderPaids_BillId",
                table: "OrderPaids");

            migrationBuilder.DropIndex(
                name: "IX_OrderPaids_BillId1",
                table: "OrderPaids");

            migrationBuilder.DropColumn(
                name: "BillId",
                table: "OrderPaids");

            migrationBuilder.DropColumn(
                name: "BillId1",
                table: "OrderPaids");
        }
    }
}
