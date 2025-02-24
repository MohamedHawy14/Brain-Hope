using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrainHope.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class AddImageToChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Instead of deleting roles, update them if needed.
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73e6c002-41d7-471b-a57c-381f0d2d617d",
                columns: new[] { "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "updated-concurrency-stamp-1", "Patient", "PATIENT" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "94879194-0e99-4492-b460-09e4d7143544",
                columns: new[] { "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "updated-concurrency-stamp-2", "Admin", "ADMIN" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a1884f1d-3eaf-46b8-b71b-26878bbb0283",
                columns: new[] { "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "updated-concurrency-stamp-3", "Doctor", "DOCTOR" });

            // Add new columns to ChatMessages table.
            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "ChatMessages",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageType",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the new columns.
            migrationBuilder.DropColumn(
                name: "Image",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "ChatMessages");

            // Optionally, you could revert the role data back to its previous values.
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73e6c002-41d7-471b-a57c-381f0d2d617d",
                columns: new[] { "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "original-stamp-1", "Patient", "PATIENT" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "94879194-0e99-4492-b460-09e4d7143544",
                columns: new[] { "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "original-stamp-2", "Admin", "ADMIN" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a1884f1d-3eaf-46b8-b71b-26878bbb0283",
                columns: new[] { "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "original-stamp-3", "Doctor", "DOCTOR" });
        }
    }
}
