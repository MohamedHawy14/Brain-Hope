using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrainHope.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpVerifiedToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<bool>(
                name: "OtpVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "OtpVerified",
                table: "AspNetUsers");

           
        }
    }
}
