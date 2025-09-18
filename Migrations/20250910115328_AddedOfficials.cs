using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipleComplaintMgmtSys.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedOfficials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Officials",
                columns: table => new
                {
                    OfficialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Officials", x => x.OfficialId);
                    table.ForeignKey(
                        name: "FK_Officials_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Officials_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Officials_DepartmentId",
                table: "Officials",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Officials_UserId",
                table: "Officials",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Officials");
        }
    }
}
