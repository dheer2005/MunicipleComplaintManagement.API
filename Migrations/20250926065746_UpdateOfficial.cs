using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipleComplaintMgmtSys.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOfficial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Officials_Departments_DepartmentId",
                table: "Officials");

            migrationBuilder.DropIndex(
                name: "IX_Officials_DepartmentId",
                table: "Officials");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Officials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Officials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Officials_DepartmentId",
                table: "Officials",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Officials_Departments_DepartmentId",
                table: "Officials",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
