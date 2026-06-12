using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampFitFurDogs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCustomerTableEmailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_customers_email",
                table: "customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_customers_email",
                table: "customers",
                column: "email",
                unique: true);
        }
    }
}
