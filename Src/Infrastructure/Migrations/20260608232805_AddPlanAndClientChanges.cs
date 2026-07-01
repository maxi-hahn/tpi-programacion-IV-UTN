using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanAndClientChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<bool>(
                name: "IsUnlimited",
                table: "Plans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id_Plan",
                table: "Users",
                column: "Id_Plan");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Plans_Id_Plan",
                table: "Users",
                column: "Id_Plan",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Plans_Id_Plan",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Id_Plan",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsUnlimited",
                table: "Plans");

            migrationBuilder.AlterColumn<int>(
                name: "Id_Plan",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
