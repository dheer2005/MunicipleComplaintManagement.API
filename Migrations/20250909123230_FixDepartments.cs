using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipleComplaintMgmtSys.API.Migrations
{
    /// <inheritdoc />
    public partial class FixDepartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Categories_CategoryId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Wards_WardId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Wards_WardId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropIndex(
                name: "IX_Users_WardId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_WardId",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "AddressLine",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "Entity",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "PayloadJson",
                table: "AuditLogs",
                newName: "ActionResult");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_DepartmentId",
                table: "Categories",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Departments_DepartmentId",
                table: "Categories",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Categories_CategoryId",
                table: "Complaints",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Departments_DepartmentId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Categories_CategoryId",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Categories_DepartmentId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "ActionResult",
                table: "AuditLogs",
                newName: "PayloadJson");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "Complaints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Entity",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    WardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WardName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.WardId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_WardId",
                table: "Users",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_WardId",
                table: "Complaints",
                column: "WardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Categories_CategoryId",
                table: "Complaints",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Wards_WardId",
                table: "Complaints",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "WardId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Wards_WardId",
                table: "Users",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "WardId");
        }
    }
}
