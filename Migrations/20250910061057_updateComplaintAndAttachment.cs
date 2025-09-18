using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipleComplaintMgmtSys.API.Migrations
{
    /// <inheritdoc />
    public partial class updateComplaintAndAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "ComplaintAttachments");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "ComplaintAttachments");

            migrationBuilder.AddColumn<int>(
                name: "AttachmentType",
                table: "ComplaintAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ComplaintAttachments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentType",
                table: "ComplaintAttachments");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ComplaintAttachments");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "ComplaintAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "ComplaintAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
