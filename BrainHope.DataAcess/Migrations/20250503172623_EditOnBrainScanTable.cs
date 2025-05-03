using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainHope.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class EditOnBrainScanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrainScanResults_AspNetUsers_PatientId",
                table: "BrainScanResults");

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "BrainScanResults",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BrainScanResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_BrainScanResults_AspNetUsers_PatientId",
                table: "BrainScanResults",
                column: "PatientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrainScanResults_AspNetUsers_PatientId",
                table: "BrainScanResults");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BrainScanResults");

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "BrainScanResults",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BrainScanResults_AspNetUsers_PatientId",
                table: "BrainScanResults",
                column: "PatientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
