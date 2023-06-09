using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinicWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedImagesToDoctors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Doctors");
        }
    }
}
