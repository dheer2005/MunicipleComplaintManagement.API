using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipleComplaintMgmtSys.API.Migrations
{
    /// <inheritdoc />
    public partial class fixedDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workers_Users_WorkerId",
                table: "Workers");

            migrationBuilder.CreateIndex(
                name: "IX_Workers_UserId",
                table: "Workers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_Users_UserId",
                table: "Workers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workers_Users_UserId",
                table: "Workers");

            migrationBuilder.DropIndex(
                name: "IX_Workers_UserId",
                table: "Workers");

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_Users_WorkerId",
                table: "Workers",
                column: "WorkerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
