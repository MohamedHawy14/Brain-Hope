using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrainHope.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendlyLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02bc54d2-e3b8-43c0-bfe1-67c9f69d7ef2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "98ee9b7d-afee-45d3-afa4-cdecf3ef8502");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ebb6852a-c968-4612-98cf-d977afe0fe11");

            migrationBuilder.AddColumn<string>(
                name: "CalendlyLink",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "06a7e2d2-aa8c-4b70-8ceb-4c058956ef98", "3", "Doctor", "Doctor" },
                    { "191c5198-5b7d-482b-88e0-fd316eabb564", "1", "Admin", "Admin" },
                    { "bb6b2047-5f3d-4424-8762-d1438c5905fa", "2", "Patient", "Patient" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "06a7e2d2-aa8c-4b70-8ceb-4c058956ef98");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "191c5198-5b7d-482b-88e0-fd316eabb564");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bb6b2047-5f3d-4424-8762-d1438c5905fa");

            migrationBuilder.DropColumn(
                name: "CalendlyLink",
                table: "Doctors");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "02bc54d2-e3b8-43c0-bfe1-67c9f69d7ef2", "3", "Doctor", "Doctor" },
                    { "98ee9b7d-afee-45d3-afa4-cdecf3ef8502", "1", "Admin", "Admin" },
                    { "ebb6852a-c968-4612-98cf-d977afe0fe11", "2", "Patient", "Patient" }
                });
        }
    }
}
