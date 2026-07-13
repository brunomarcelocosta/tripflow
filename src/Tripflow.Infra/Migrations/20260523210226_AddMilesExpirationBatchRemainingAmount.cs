using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tripflow.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddMilesExpirationBatchRemainingAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingAmount",
                table: "miles_expiration_batches",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingAmount",
                table: "miles_expiration_batches");
        }
    }
}
