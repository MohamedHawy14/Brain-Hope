using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrainHope.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0bf92d39-52a6-4135-bded-b30413a75143", "2", "Patient", "Patient" },
                    { "950aac38-e2c6-47ce-b3fb-d7f375f75f81", "3", "Doctor", "Doctor" },
                    { "9ddeddff-cb7e-487f-923c-5d0a7b58532e", "1", "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0bf92d39-52a6-4135-bded-b30413a75143");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "950aac38-e2c6-47ce-b3fb-d7f375f75f81");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9ddeddff-cb7e-487f-923c-5d0a7b58532e");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "AspNetUsers");

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
    }
}
