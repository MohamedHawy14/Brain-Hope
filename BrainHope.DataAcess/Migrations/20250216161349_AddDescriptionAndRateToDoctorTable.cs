using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrainHope.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionAndRateToDoctorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6485a22e-564c-4d56-af78-6df2d52f4e4a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6fc5c1d4-440f-416f-b825-68b7ff2a2c0d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ae06d3ae-fe6c-434d-8f18-ce7314a4ee32");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "17a4fb4b-e134-4a14-860f-904d35b4d63e", "3", "Doctor", "Doctor" },
                    { "725e6973-ee1a-4079-8cc8-546be4d9757e", "1", "Admin", "Admin" },
                    { "e5870a69-6d03-420b-a6a1-35da6d8f54bc", "2", "Patient", "Patient" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "17a4fb4b-e134-4a14-860f-904d35b4d63e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "725e6973-ee1a-4079-8cc8-546be4d9757e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e5870a69-6d03-420b-a6a1-35da6d8f54bc");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Doctors");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6485a22e-564c-4d56-af78-6df2d52f4e4a", "2", "Patient", "Patient" },
                    { "6fc5c1d4-440f-416f-b825-68b7ff2a2c0d", "3", "Doctor", "Doctor" },
                    { "ae06d3ae-fe6c-434d-8f18-ce7314a4ee32", "1", "Admin", "Admin" }
                });
        }
    }
}
