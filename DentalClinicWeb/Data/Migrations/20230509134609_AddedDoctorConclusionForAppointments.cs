using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinicWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDoctorConclusionForAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoctorConclusion",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorConclusion",
                table: "Appointments");
        }
    }
}
