using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampFitFurDogs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RequiredUniqueCustomerTableExternalIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "external_auth_provider_id",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "password_hash",
                table: "customers");

            migrationBuilder.AddColumn<string>(
                name: "external_id",
                table: "customers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_customers_external_id",
                table: "customers",
                column: "external_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_customers_external_id",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "external_id",
                table: "customers");

            migrationBuilder.AddColumn<string>(
                name: "external_auth_provider_id",
                table: "customers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                table: "customers",
                type: "text",
                nullable: true);
        }
    }
}
