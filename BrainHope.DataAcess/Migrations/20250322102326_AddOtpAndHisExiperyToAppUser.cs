using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrainHope.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpAndHisExiperyToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiration",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetOtp",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
         

            migrationBuilder.DropColumn(
                name: "OtpExpiration",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ResetOtp",
                table: "AspNetUsers");

           
        }
    }
}
