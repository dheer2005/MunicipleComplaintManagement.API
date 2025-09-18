using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipleComplaintMgmtSys.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedWorkHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkUpdateId",
                table: "ComplaintAttachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkUpdates",
                columns: table => new
                {
                    WorkUpdateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComplaintId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletionPercentage = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiresAdditionalResources = table.Column<bool>(type: "bit", nullable: false),
                    AdditionalResourcesNeeded = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkUpdates", x => x.WorkUpdateId);
                    table.ForeignKey(
                        name: "FK_WorkUpdates_Complaints_ComplaintId",
                        column: x => x.ComplaintId,
                        principalTable: "Complaints",
                        principalColumn: "ComplaintId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkUpdates_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintAttachments_WorkUpdateId",
                table: "ComplaintAttachments",
                column: "WorkUpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkUpdates_ComplaintId",
                table: "WorkUpdates",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkUpdates_UpdatedByUserId",
                table: "WorkUpdates",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintAttachments_WorkUpdates_WorkUpdateId",
                table: "ComplaintAttachments",
                column: "WorkUpdateId",
                principalTable: "WorkUpdates",
                principalColumn: "WorkUpdateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintAttachments_WorkUpdates_WorkUpdateId",
                table: "ComplaintAttachments");

            migrationBuilder.DropTable(
                name: "WorkUpdates");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintAttachments_WorkUpdateId",
                table: "ComplaintAttachments");

            migrationBuilder.DropColumn(
                name: "WorkUpdateId",
                table: "ComplaintAttachments");
        }
    }
}
