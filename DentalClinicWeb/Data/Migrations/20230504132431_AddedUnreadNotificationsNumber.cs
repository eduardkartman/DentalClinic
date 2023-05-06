using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinicWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedUnreadNotificationsNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnreadNotifications",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnreadNotifications",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnreadNotifications",
                table: "Doctors",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnreadNotifications",
                table: "AspNetUsers",
                type: "int",
                maxLength: 3,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnreadNotifications",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UnreadNotifications",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UnreadNotifications",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "UnreadNotifications",
                table: "AspNetUsers");
        }
    }
}
