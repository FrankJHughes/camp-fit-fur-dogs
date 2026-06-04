using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampFitFurDogs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRevokedAtColumnToSessionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "revoked_at",
                table: "sessions",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "revoked_at",
                table: "sessions");
        }
    }
}
