using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tripflow.Infra.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        private static readonly string[] columns = new[] { "Id", "AirlineName", "Country", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "DeletedBy", "IsDeleted", "Name", "Status", "UpdatedAtUtc", "UpdatedBy" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "loyalty_programs",
                columns: columns,
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666660001"), "GOL", "Brazil", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, false, "Smiles", "Active", null, null },
                    { new Guid("66666666-6666-6666-6666-666666660002"), "LATAM", "Brazil", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, false, "LATAM Pass", "Active", null, null },
                    { new Guid("66666666-6666-6666-6666-666666660003"), "Azul", "Brazil", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, false, "TudoAzul", "Active", null, null },
                    { new Guid("66666666-6666-6666-6666-666666660004"), null, "Brazil", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, false, "Livelo", "Active", null, null },
                    { new Guid("66666666-6666-6666-6666-666666660005"), null, "Brazil", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, false, "Esfera", "Active", null, null },
                    { new Guid("66666666-6666-6666-6666-666666660006"), "TAP", "Portugal", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, false, "TAP Miles&Go", "Active", null, null },
                    { new Guid("66666666-6666-6666-6666-666666660007"), "American Airlines", "United States", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, false, "AAdvantage", "Active", null, null },
                    { new Guid("66666666-6666-6666-6666-666666660008"), "Air Canada", "Canada", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, false, "Aeroplan", "Active", null, null },
                    { new Guid("66666666-6666-6666-6666-666666660009"), "Air France-KLM", "France/Netherlands", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, false, "Flying Blue", "Active", null, null }
                });

            migrationBuilder.InsertData(
                table: "payment_providers",
                columns: columnsArray,
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555550001"), "manual", "Manual", "Active" },
                    { new Guid("55555555-5555-5555-5555-555555550002"), "asaas", "Asaas", "Active" },
                    { new Guid("55555555-5555-5555-5555-555555550003"), "pagarme", "Pagar.me", "Active" },
                    { new Guid("55555555-5555-5555-5555-555555550004"), "infinitepay", "InfinitePay", "Active" },
                    { new Guid("55555555-5555-5555-5555-555555550005"), "mercadopago", "Mercado Pago", "Active" },
                    { new Guid("55555555-5555-5555-5555-555555550006"), "stripe", "Stripe", "Active" }
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333330001"), "tenants.read", "Visualizar empresas/tenants." },
                    { new Guid("33333333-3333-3333-3333-333333330002"), "tenants.write", "Criar e alterar empresas/tenants." },
                    { new Guid("33333333-3333-3333-3333-333333330003"), "users.read", "Visualizar usuários." },
                    { new Guid("33333333-3333-3333-3333-333333330004"), "users.manage", "Gerenciar usuários, convites e acessos." },
                    { new Guid("33333333-3333-3333-3333-333333330005"), "settings.read", "Visualizar configurações da empresa." },
                    { new Guid("33333333-3333-3333-3333-333333330006"), "settings.manage", "Gerenciar configurações da empresa." },
                    { new Guid("33333333-3333-3333-3333-333333330007"), "customers.read", "Visualizar clientes." },
                    { new Guid("33333333-3333-3333-3333-333333330008"), "customers.write", "Criar e alterar clientes." },
                    { new Guid("33333333-3333-3333-3333-333333330009"), "travelers.read", "Visualizar viajantes." },
                    { new Guid("33333333-3333-3333-3333-333333330010"), "travelers.write", "Criar e alterar viajantes." },
                    { new Guid("33333333-3333-3333-3333-333333330011"), "quotes.read", "Visualizar cotações." },
                    { new Guid("33333333-3333-3333-3333-333333330012"), "quotes.write", "Criar e alterar cotações." },
                    { new Guid("33333333-3333-3333-3333-333333330013"), "quotes.approve", "Aprovar cotações internamente." },
                    { new Guid("33333333-3333-3333-3333-333333330014"), "proposals.read", "Visualizar propostas." },
                    { new Guid("33333333-3333-3333-3333-333333330015"), "proposals.write", "Criar e alterar propostas." },
                    { new Guid("33333333-3333-3333-3333-333333330016"), "proposals.send", "Enviar propostas ao cliente." },
                    { new Guid("33333333-3333-3333-3333-333333330017"), "payments.read", "Visualizar pagamentos." },
                    { new Guid("33333333-3333-3333-3333-333333330018"), "payments.write", "Criar e alterar pagamentos." },
                    { new Guid("33333333-3333-3333-3333-333333330019"), "miles.read", "Visualizar dados de milhas." },
                    { new Guid("33333333-3333-3333-3333-333333330020"), "miles.write", "Criar e alterar dados de milhas." },
                    { new Guid("33333333-3333-3333-3333-333333330021"), "reports.read", "Visualizar relatórios." },
                    { new Guid("33333333-3333-3333-3333-333333330022"), "audit.read", "Visualizar auditoria." }
                });

            migrationBuilder.InsertData(
                table: "subscription_plans",
                columns: new[] { "Id", "AnnualPrice", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "DeletedBy", "Description", "IsDeleted", "MaxQuotesPerMonth", "MaxUsers", "MonthlyPrice", "Name", "Status", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222221"), 499.00m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Plano inicial para consultores individuais.", false, 30, 1, 49.90m, "Starter", "Active", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 1499.00m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Plano para pequenas agências.", false, 200, 5, 149.90m, "Agency", "Active", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222223"), 2999.00m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Plano profissional para operações maiores.", false, 1000, 15, 299.90m, "Professional", "Active", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222224"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Plano enterprise com limites customizados.", false, null, null, null, "Enterprise", "Active", null, null }
                });

            migrationBuilder.InsertData(
                table: "tenants",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "DeletedBy", "DocumentNumber", "Email", "IsDeleted", "LegalName", "Phone", "Status", "TradeName", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, null, "admin@tripflow.com", false, "TripFlow Platform", null, "Active", "TripFlow", null, null });

            migrationBuilder.InsertData(
                table: "payment_fee_rules",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "DeletedBy", "FeePercentage", "Installments", "IsActive", "IsDeleted", "PaymentMethod", "TenantId", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777770001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 4.20m, 1, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 6.09m, 2, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 7.01m, 3, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 7.91m, 4, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 8.80m, 5, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 9.67m, 6, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 12.59m, 7, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770008"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 13.42m, 8, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770009"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 14.25m, 9, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770010"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 15.06m, 10, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770011"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 15.87m, 11, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770012"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 16.66m, 12, true, false, "CreditCard", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770013"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 0.00m, 1, true, false, "Pix", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777770014"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, 0.00m, 1, true, false, "Manual", new Guid("11111111-1111-1111-1111-111111111111"), null, null }
                });

            migrationBuilder.InsertData(
                table: "plan_features",
                columns: new[] { "Id", "Enabled", "FeatureCode", "LimitValue", "SubscriptionPlanId" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888880001"), true, "crm", null, new Guid("22222222-2222-2222-2222-222222222221") },
                    { new Guid("88888888-8888-8888-8888-888888880002"), true, "quotes", 30, new Guid("22222222-2222-2222-2222-222222222221") },
                    { new Guid("88888888-8888-8888-8888-888888880003"), true, "pricing", null, new Guid("22222222-2222-2222-2222-222222222221") },
                    { new Guid("88888888-8888-8888-8888-888888880004"), true, "proposal_pdf", null, new Guid("22222222-2222-2222-2222-222222222221") },
                    { new Guid("88888888-8888-8888-8888-888888880005"), true, "crm", null, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("88888888-8888-8888-8888-888888880006"), true, "quotes", 200, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("88888888-8888-8888-8888-888888880007"), true, "pricing", null, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("88888888-8888-8888-8888-888888880008"), true, "proposal_pdf", null, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("88888888-8888-8888-8888-888888880009"), true, "customer_portal", null, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("88888888-8888-8888-8888-888888880010"), true, "payments", null, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("88888888-8888-8888-8888-888888880011"), true, "branding", null, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("88888888-8888-8888-8888-888888880012"), true, "crm", null, new Guid("22222222-2222-2222-2222-222222222223") },
                    { new Guid("88888888-8888-8888-8888-888888880013"), true, "quotes", 1000, new Guid("22222222-2222-2222-2222-222222222223") },
                    { new Guid("88888888-8888-8888-8888-888888880014"), true, "pricing", null, new Guid("22222222-2222-2222-2222-222222222223") },
                    { new Guid("88888888-8888-8888-8888-888888880015"), true, "proposal_pdf", null, new Guid("22222222-2222-2222-2222-222222222223") },
                    { new Guid("88888888-8888-8888-8888-888888880016"), true, "customer_portal", null, new Guid("22222222-2222-2222-2222-222222222223") },
                    { new Guid("88888888-8888-8888-8888-888888880017"), true, "payments", null, new Guid("22222222-2222-2222-2222-222222222223") },
                    { new Guid("88888888-8888-8888-8888-888888880018"), true, "miles", null, new Guid("22222222-2222-2222-2222-222222222223") },
                    { new Guid("88888888-8888-8888-8888-888888880019"), true, "reports", null, new Guid("22222222-2222-2222-2222-222222222223") },
                    { new Guid("88888888-8888-8888-8888-888888880020"), true, "branding", null, new Guid("22222222-2222-2222-2222-222222222223") },
                    { new Guid("88888888-8888-8888-8888-888888880021"), true, "crm", null, new Guid("22222222-2222-2222-2222-222222222224") },
                    { new Guid("88888888-8888-8888-8888-888888880022"), true, "quotes", null, new Guid("22222222-2222-2222-2222-222222222224") },
                    { new Guid("88888888-8888-8888-8888-888888880023"), true, "pricing", null, new Guid("22222222-2222-2222-2222-222222222224") },
                    { new Guid("88888888-8888-8888-8888-888888880024"), true, "proposal_pdf", null, new Guid("22222222-2222-2222-2222-222222222224") },
                    { new Guid("88888888-8888-8888-8888-888888880025"), true, "customer_portal", null, new Guid("22222222-2222-2222-2222-222222222224") },
                    { new Guid("88888888-8888-8888-8888-888888880026"), true, "payments", null, new Guid("22222222-2222-2222-2222-222222222224") },
                    { new Guid("88888888-8888-8888-8888-888888880027"), true, "miles", null, new Guid("22222222-2222-2222-2222-222222222224") },
                    { new Guid("88888888-8888-8888-8888-888888880028"), true, "reports", null, new Guid("22222222-2222-2222-2222-222222222224") },
                    { new Guid("88888888-8888-8888-8888-888888880029"), true, "branding", null, new Guid("22222222-2222-2222-2222-222222222224") },
                    { new Guid("88888888-8888-8888-8888-888888880030"), true, "white_label", null, new Guid("22222222-2222-2222-2222-222222222224") }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "DeletedBy", "Description", "IsDeleted", "IsSystemRole", "Name", "TenantId", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444440001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Administrador global da plataforma.", false, true, "PlatformAdmin", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Dono da empresa/agência.", false, true, "TenantOwner", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Administrador da agência.", false, true, "AgencyAdmin", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Consultor de viagens.", false, true, "Consultant", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Usuário financeiro.", false, true, "Financial", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Operador de emissão e operação.", false, true, "Operator", new Guid("11111111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "Usuário com acesso somente leitura.", false, true, "CustomerViewer", new Guid("11111111-1111-1111-1111-111111111111"), null, null }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333330001"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330002"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330003"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330004"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330005"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330006"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330008"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330010"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330012"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330013"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330015"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330016"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330018"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330020"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330021"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330022"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330003"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330004"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330005"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330006"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330008"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330010"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330012"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330013"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330015"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330016"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330018"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330020"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330021"), new Guid("44444444-4444-4444-4444-444444440002") },
                    { new Guid("33333333-3333-3333-3333-333333330003"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330004"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330005"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330006"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330008"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330010"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330012"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330013"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330015"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330016"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330018"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330020"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330021"), new Guid("44444444-4444-4444-4444-444444440003") },
                    { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440004") },
                    { new Guid("33333333-3333-3333-3333-333333330008"), new Guid("44444444-4444-4444-4444-444444440004") },
                    { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440004") },
                    { new Guid("33333333-3333-3333-3333-333333330010"), new Guid("44444444-4444-4444-4444-444444440004") },
                    { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440004") },
                    { new Guid("33333333-3333-3333-3333-333333330012"), new Guid("44444444-4444-4444-4444-444444440004") },
                    { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440004") },
                    { new Guid("33333333-3333-3333-3333-333333330015"), new Guid("44444444-4444-4444-4444-444444440004") },
                    { new Guid("33333333-3333-3333-3333-333333330016"), new Guid("44444444-4444-4444-4444-444444440004") },
                    { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440004") },
                    { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440005") },
                    { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440005") },
                    { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440005") },
                    { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440005") },
                    { new Guid("33333333-3333-3333-3333-333333330018"), new Guid("44444444-4444-4444-4444-444444440005") },
                    { new Guid("33333333-3333-3333-3333-333333330021"), new Guid("44444444-4444-4444-4444-444444440005") },
                    { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440006") },
                    { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440006") },
                    { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440006") },
                    { new Guid("33333333-3333-3333-3333-333333330012"), new Guid("44444444-4444-4444-4444-444444440006") },
                    { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440006") },
                    { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440006") },
                    { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440006") },
                    { new Guid("33333333-3333-3333-3333-333333330020"), new Guid("44444444-4444-4444-4444-444444440006") },
                    { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440007") },
                    { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440007") },
                    { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440007") },
                    { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440007") },
                    { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440007") },
                    { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440007") }
                });
        }

        private static readonly string[] keyColumns = new[] { "PermissionId", "RoleId" };
        private static readonly string[] columnsArray = new[] { "Id", "Code", "Name", "Status" };

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "loyalty_programs",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666660001"));

            migrationBuilder.DeleteData(
                table: "loyalty_programs",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666660002"));

            migrationBuilder.DeleteData(
                table: "loyalty_programs",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666660003"));

            migrationBuilder.DeleteData(
                table: "loyalty_programs",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666660004"));

            migrationBuilder.DeleteData(
                table: "loyalty_programs",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666660005"));

            migrationBuilder.DeleteData(
                table: "loyalty_programs",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666660006"));

            migrationBuilder.DeleteData(
                table: "loyalty_programs",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666660007"));

            migrationBuilder.DeleteData(
                table: "loyalty_programs",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666660008"));

            migrationBuilder.DeleteData(
                table: "loyalty_programs",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666660009"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770001"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770002"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770003"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770004"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770005"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770006"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770007"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770008"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770009"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770010"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770011"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770012"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770013"));

            migrationBuilder.DeleteData(
                table: "payment_fee_rules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777770014"));

            migrationBuilder.DeleteData(
                table: "payment_providers",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555550001"));

            migrationBuilder.DeleteData(
                table: "payment_providers",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555550002"));

            migrationBuilder.DeleteData(
                table: "payment_providers",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555550003"));

            migrationBuilder.DeleteData(
                table: "payment_providers",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555550004"));

            migrationBuilder.DeleteData(
                table: "payment_providers",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555550005"));

            migrationBuilder.DeleteData(
                table: "payment_providers",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555550006"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880001"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880002"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880003"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880004"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880005"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880006"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880007"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880008"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880009"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880010"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880011"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880012"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880013"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880014"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880015"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880016"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880017"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880018"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880019"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880020"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880021"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880022"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880023"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880024"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880025"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880026"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880027"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880028"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880029"));

            migrationBuilder.DeleteData(
                table: "plan_features",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888880030"));

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: keyColumns,
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330001"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: keyColumns,
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330002"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330003"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330004"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330005"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330006"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330008"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330010"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330012"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330013"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330015"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330016"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330018"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330020"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330021"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330022"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330003"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330004"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330005"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330006"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330008"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330010"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330012"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330013"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330015"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330016"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330018"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330020"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330021"), new Guid("44444444-4444-4444-4444-444444440002") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330003"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330004"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330005"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330006"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330008"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330010"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330012"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330013"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330015"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330016"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330018"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330020"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330021"), new Guid("44444444-4444-4444-4444-444444440003") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440004") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330008"), new Guid("44444444-4444-4444-4444-444444440004") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440004") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330010"), new Guid("44444444-4444-4444-4444-444444440004") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440004") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330012"), new Guid("44444444-4444-4444-4444-444444440004") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440004") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330015"), new Guid("44444444-4444-4444-4444-444444440004") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330016"), new Guid("44444444-4444-4444-4444-444444440004") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440004") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440005") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440005") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440005") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440005") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330018"), new Guid("44444444-4444-4444-4444-444444440005") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330021"), new Guid("44444444-4444-4444-4444-444444440005") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440006") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440006") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440006") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330012"), new Guid("44444444-4444-4444-4444-444444440006") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440006") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440006") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440006") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330020"), new Guid("44444444-4444-4444-4444-444444440006") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330007"), new Guid("44444444-4444-4444-4444-444444440007") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330009"), new Guid("44444444-4444-4444-4444-444444440007") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330011"), new Guid("44444444-4444-4444-4444-444444440007") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330014"), new Guid("44444444-4444-4444-4444-444444440007") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330017"), new Guid("44444444-4444-4444-4444-444444440007") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330019"), new Guid("44444444-4444-4444-4444-444444440007") });

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330001"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330002"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330003"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330004"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330005"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330006"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330007"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330008"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330009"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330010"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330011"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330012"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330013"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330014"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330015"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330016"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330017"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330018"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330019"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330020"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330021"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330022"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444440001"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444440002"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444440003"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444440004"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444440005"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444440006"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444440007"));

            migrationBuilder.DeleteData(
                table: "subscription_plans",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"));

            migrationBuilder.DeleteData(
                table: "subscription_plans",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "subscription_plans",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"));

            migrationBuilder.DeleteData(
                table: "subscription_plans",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222224"));

            migrationBuilder.DeleteData(
                table: "tenants",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
