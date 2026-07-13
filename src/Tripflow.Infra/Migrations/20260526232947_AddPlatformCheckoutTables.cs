using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tripflow.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformCheckoutTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAtUtc",
                table: "leads",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "leads",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PlatformCheckoutSessionId",
                table: "leads",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "leads",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubscriptionPlanId",
                table: "leads",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "platform_checkout_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LeadId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalCheckoutId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CheckoutUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaidAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RawProviderResponse = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_checkout_sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "platform_payment_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExternalEventId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExternalCheckoutId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    Processed = table.Column<bool>(type: "boolean", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_payment_events", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_leads_PaymentStatus",
                table: "leads",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_leads_SubscriptionPlanId",
                table: "leads",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_platform_checkout_sessions_CreatedAtUtc",
                table: "platform_checkout_sessions",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_platform_checkout_sessions_ExternalCheckoutId",
                table: "platform_checkout_sessions",
                column: "ExternalCheckoutId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_platform_checkout_sessions_LeadId",
                table: "platform_checkout_sessions",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_platform_checkout_sessions_Status",
                table: "platform_checkout_sessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_platform_checkout_sessions_SubscriptionPlanId",
                table: "platform_checkout_sessions",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_platform_payment_events_ExternalCheckoutId",
                table: "platform_payment_events",
                column: "ExternalCheckoutId");

            migrationBuilder.CreateIndex(
                name: "IX_platform_payment_events_Processed",
                table: "platform_payment_events",
                column: "Processed");

            migrationBuilder.CreateIndex(
                name: "IX_platform_payment_events_ProviderCode_ExternalEventId",
                table: "platform_payment_events",
                columns: new[] { "ProviderCode", "ExternalEventId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "platform_checkout_sessions");

            migrationBuilder.DropTable(
                name: "platform_payment_events");

            migrationBuilder.DropIndex(
                name: "IX_leads_PaymentStatus",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_SubscriptionPlanId",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "PaidAtUtc",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "PlatformCheckoutSessionId",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanId",
                table: "leads");
        }
    }
}
