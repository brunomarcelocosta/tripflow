using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tripflow.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddTravelerPassportExpirationIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_travelers_TenantId_PassportExpirationDate",
                table: "travelers",
                columns: new[] { "TenantId", "PassportExpirationDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_travelers_TenantId_PassportExpirationDate",
                table: "travelers");
        }
    }
}
