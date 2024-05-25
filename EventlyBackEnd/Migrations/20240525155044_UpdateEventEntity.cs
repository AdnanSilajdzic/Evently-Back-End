using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventlyBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Organizer",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Organizer",
                table: "Events");
        }
    }
}
