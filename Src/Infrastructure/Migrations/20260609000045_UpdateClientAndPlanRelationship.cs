using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClientAndPlanRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Plans_Id_Plan",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Plans_Id_Plan",
                table: "Users",
                column: "Id_Plan",
                principalTable: "Plans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Plans_Id_Plan",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Plans_Id_Plan",
                table: "Users",
                column: "Id_Plan",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
