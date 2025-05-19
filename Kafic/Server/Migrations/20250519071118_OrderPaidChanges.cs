using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class OrderPaidChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderPaids_Bills_BillId1",
                table: "OrderPaids");

            migrationBuilder.DropIndex(
                name: "IX_OrderPaids_BillId1",
                table: "OrderPaids");

            migrationBuilder.DropColumn(
                name: "BillId1",
                table: "OrderPaids");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillId1",
                table: "OrderPaids",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaids_BillId1",
                table: "OrderPaids",
                column: "BillId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderPaids_Bills_BillId1",
                table: "OrderPaids",
                column: "BillId1",
                principalTable: "Bills",
                principalColumn: "Id");
        }
    }
}
