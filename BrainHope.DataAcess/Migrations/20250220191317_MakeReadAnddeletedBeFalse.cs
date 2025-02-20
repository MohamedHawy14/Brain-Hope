using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrainHope.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class MakeReadAnddeletedBeFalse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1960ffb7-e372-42b2-8881-9925a9e6bbfd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "42b79b13-2036-4488-b742-1bb3eac122d8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b4c71167-0aaa-463a-a66a-56aaf921a00d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3c4fdab9-71ea-41a1-b817-e11409885f77", "1", "Admin", "Admin" },
                    { "a783e12f-e35f-4895-8919-3890d7e8ecca", "2", "Patient", "Patient" },
                    { "c2d8ef10-04cd-447e-bd74-ba0ca211d697", "3", "Doctor", "Doctor" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c4fdab9-71ea-41a1-b817-e11409885f77");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a783e12f-e35f-4895-8919-3890d7e8ecca");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c2d8ef10-04cd-447e-bd74-ba0ca211d697");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1960ffb7-e372-42b2-8881-9925a9e6bbfd", "3", "Doctor", "Doctor" },
                    { "42b79b13-2036-4488-b742-1bb3eac122d8", "2", "Patient", "Patient" },
                    { "b4c71167-0aaa-463a-a66a-56aaf921a00d", "1", "Admin", "Admin" }
                });
        }
    }
}
