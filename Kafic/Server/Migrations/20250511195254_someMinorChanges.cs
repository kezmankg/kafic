using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class someMinorChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Subgroups_SubgroupId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Subgroups_Groups_GroupId",
                table: "Subgroups");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Subgroups_SubgroupId",
                table: "Articles",
                column: "SubgroupId",
                principalTable: "Subgroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subgroups_Groups_GroupId",
                table: "Subgroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Subgroups_SubgroupId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Subgroups_Groups_GroupId",
                table: "Subgroups");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Subgroups_SubgroupId",
                table: "Articles",
                column: "SubgroupId",
                principalTable: "Subgroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subgroups_Groups_GroupId",
                table: "Subgroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
