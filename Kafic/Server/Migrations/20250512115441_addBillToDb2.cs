using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class addBillToDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_Caffes_CaffeId",
                table: "Bill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bill",
                table: "Bill");

            migrationBuilder.RenameTable(
                name: "Bill",
                newName: "Bills");

            migrationBuilder.RenameIndex(
                name: "IX_Bill_CaffeId",
                table: "Bills",
                newName: "IX_Bills_CaffeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bills",
                table: "Bills",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Caffes_CaffeId",
                table: "Bills",
                column: "CaffeId",
                principalTable: "Caffes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Caffes_CaffeId",
                table: "Bills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bills",
                table: "Bills");

            migrationBuilder.RenameTable(
                name: "Bills",
                newName: "Bill");

            migrationBuilder.RenameIndex(
                name: "IX_Bills_CaffeId",
                table: "Bill",
                newName: "IX_Bill_CaffeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bill",
                table: "Bill",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_Caffes_CaffeId",
                table: "Bill",
                column: "CaffeId",
                principalTable: "Caffes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
