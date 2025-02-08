using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrainHope.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7aaf4bdd-57b6-4b13-ac44-e818237c35f5", "1", "Admin", "Admin" },
                    { "b35991c2-5476-4203-8895-c824c973927c", "2", "Patient", "Patient" },
                    { "d0ed79d7-bf88-4918-a99f-27e176e9552f", "3", "Doctor", "Doctor" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7aaf4bdd-57b6-4b13-ac44-e818237c35f5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b35991c2-5476-4203-8895-c824c973927c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d0ed79d7-bf88-4918-a99f-27e176e9552f");
        }
    }
}
