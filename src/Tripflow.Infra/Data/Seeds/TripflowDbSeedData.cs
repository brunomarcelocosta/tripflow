using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Enums;

namespace Tripflow.Infra.Data.Seeds;

/// <summary>
/// Dados iniciais aplicados via <see cref="ModelBuilder"/> (HasData).
/// Gere a migration com: dotnet ef migrations add SeedInitialData --project src/Tripflow.Infra
/// </summary>
public static class TripflowDbSeedData
{
    public static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid PlatformTenantId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid StarterPlanId =
        Guid.Parse("22222222-2222-2222-2222-222222222221");

    public static readonly Guid AgencyPlanId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static readonly Guid ProfessionalPlanId =
        Guid.Parse("22222222-2222-2222-2222-222222222223");

    public static readonly Guid EnterprisePlanId =
        Guid.Parse("22222222-2222-2222-2222-222222222224");

    private static readonly Guid RolePlatformAdminId =
        Guid.Parse("44444444-4444-4444-4444-444444440001");

    private static readonly Guid RoleTenantOwnerId =
        Guid.Parse("44444444-4444-4444-4444-444444440002");

    private static readonly Guid RoleAgencyAdminId =
        Guid.Parse("44444444-4444-4444-4444-444444440003");

    private static readonly Guid RoleConsultantId =
        Guid.Parse("44444444-4444-4444-4444-444444440004");

    private static readonly Guid RoleFinancialId =
        Guid.Parse("44444444-4444-4444-4444-444444440005");

    private static readonly Guid RoleOperatorId =
        Guid.Parse("44444444-4444-4444-4444-444444440006");

    private static readonly Guid RoleCustomerViewerId =
        Guid.Parse("44444444-4444-4444-4444-444444440007");

    private static readonly Dictionary<string, Guid> PermissionIds = new()
    {
        [Permissions.TenantsRead] = Guid.Parse("33333333-3333-3333-3333-333333330001"),
        [Permissions.TenantsWrite] = Guid.Parse("33333333-3333-3333-3333-333333330002"),
        [Permissions.UsersRead] = Guid.Parse("33333333-3333-3333-3333-333333330003"),
        [Permissions.UsersManage] = Guid.Parse("33333333-3333-3333-3333-333333330004"),
        [Permissions.SettingsRead] = Guid.Parse("33333333-3333-3333-3333-333333330005"),
        [Permissions.SettingsManage] = Guid.Parse("33333333-3333-3333-3333-333333330006"),
        [Permissions.CustomersRead] = Guid.Parse("33333333-3333-3333-3333-333333330007"),
        [Permissions.CustomersWrite] = Guid.Parse("33333333-3333-3333-3333-333333330008"),
        [Permissions.TravelersRead] = Guid.Parse("33333333-3333-3333-3333-333333330009"),
        [Permissions.TravelersWrite] = Guid.Parse("33333333-3333-3333-3333-333333330010"),
        [Permissions.QuotesRead] = Guid.Parse("33333333-3333-3333-3333-333333330011"),
        [Permissions.QuotesWrite] = Guid.Parse("33333333-3333-3333-3333-333333330012"),
        [Permissions.QuotesApprove] = Guid.Parse("33333333-3333-3333-3333-333333330013"),
        [Permissions.ProposalsRead] = Guid.Parse("33333333-3333-3333-3333-333333330014"),
        [Permissions.ProposalsWrite] = Guid.Parse("33333333-3333-3333-3333-333333330015"),
        [Permissions.ProposalsSend] = Guid.Parse("33333333-3333-3333-3333-333333330016"),
        [Permissions.PaymentsRead] = Guid.Parse("33333333-3333-3333-3333-333333330017"),
        [Permissions.PaymentsWrite] = Guid.Parse("33333333-3333-3333-3333-333333330018"),
        [Permissions.MilesRead] = Guid.Parse("33333333-3333-3333-3333-333333330019"),
        [Permissions.MilesWrite] = Guid.Parse("33333333-3333-3333-3333-333333330020"),
        [Permissions.ReportsRead] = Guid.Parse("33333333-3333-3333-3333-333333330021"),
        [Permissions.AuditRead] = Guid.Parse("33333333-3333-3333-3333-333333330022"),
        [PlatformPermissions.DashboardRead] = Guid.Parse("33333333-3333-3333-3333-333333330023"),
        [PlatformPermissions.TenantsRead] = Guid.Parse("33333333-3333-3333-3333-333333330024"),
        [PlatformPermissions.TenantsManage] = Guid.Parse("33333333-3333-3333-3333-333333330025"),
        [PlatformPermissions.UsersRead] = Guid.Parse("33333333-3333-3333-3333-333333330026"),
        [PlatformPermissions.UsersManage] = Guid.Parse("33333333-3333-3333-3333-333333330027"),
        [PlatformPermissions.ReportsRead] = Guid.Parse("33333333-3333-3333-3333-333333330028"),
        [PlatformPermissions.SupportAccess] = Guid.Parse("33333333-3333-3333-3333-333333330029"),
        [PlatformPermissions.AuditRead] = Guid.Parse("33333333-3333-3333-3333-333333330030"),
    };

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Tenants.Tenant>().HasData(
            new
            {
                Id = PlatformTenantId,
                LegalName = "TripFlow Platform",
                TradeName = "TripFlow",
                DocumentNumber = (string?)null,
                Email = "admin@tripflow.com",
                Phone = (string?)null,
                Status = TenantStatus.Active,
                CreatedAtUtc = SeedDate,
                CreatedBy = "system",
                IsDeleted = false
            });

        modelBuilder.Entity<Domain.Entities.Identity.Permission>().HasData(GetPermissions());

        modelBuilder.Entity<Domain.Entities.Identity.Role>().HasData(GetRoles());

        modelBuilder.Entity<Domain.Entities.Identity.RolePermission>().HasData(GetRolePermissions());

        modelBuilder.Entity<Domain.Entities.Payments.PaymentProvider>().HasData(GetPaymentProviders());

        modelBuilder.Entity<Domain.Entities.Subscriptions.SubscriptionPlan>().HasData(GetSubscriptionPlans());

        modelBuilder.Entity<Domain.Entities.Subscriptions.PlanFeature>().HasData(GetPlanFeatures());

        modelBuilder.Entity<Domain.Entities.Miles.LoyaltyProgram>().HasData(GetLoyaltyPrograms());

        modelBuilder.Entity<Domain.Entities.Pricing.PaymentFeeRule>().HasData(GetPaymentFeeRules());
    }

    private static object[] GetPermissions() =>
    [
        Perm(Permissions.TenantsRead, "Visualizar empresas/tenants."),
        Perm(Permissions.TenantsWrite, "Criar e alterar empresas/tenants."),
        Perm(Permissions.UsersRead, "Visualizar usuários."),
        Perm(Permissions.UsersManage, "Gerenciar usuários, convites e acessos."),
        Perm(Permissions.SettingsRead, "Visualizar configurações da empresa."),
        Perm(Permissions.SettingsManage, "Gerenciar configurações da empresa."),
        Perm(Permissions.CustomersRead, "Visualizar clientes."),
        Perm(Permissions.CustomersWrite, "Criar e alterar clientes."),
        Perm(Permissions.TravelersRead, "Visualizar viajantes."),
        Perm(Permissions.TravelersWrite, "Criar e alterar viajantes."),
        Perm(Permissions.QuotesRead, "Visualizar cotações."),
        Perm(Permissions.QuotesWrite, "Criar e alterar cotações."),
        Perm(Permissions.QuotesApprove, "Aprovar cotações internamente."),
        Perm(Permissions.ProposalsRead, "Visualizar propostas."),
        Perm(Permissions.ProposalsWrite, "Criar e alterar propostas."),
        Perm(Permissions.ProposalsSend, "Enviar propostas ao cliente."),
        Perm(Permissions.PaymentsRead, "Visualizar pagamentos."),
        Perm(Permissions.PaymentsWrite, "Criar e alterar pagamentos."),
        Perm(Permissions.MilesRead, "Visualizar dados de milhas."),
        Perm(Permissions.MilesWrite, "Criar e alterar dados de milhas."),
        Perm(Permissions.ReportsRead, "Visualizar relatórios."),
        Perm(Permissions.AuditRead, "Visualizar auditoria."),
        Perm(PlatformPermissions.DashboardRead, "Visualizar dashboard administrativo da plataforma."),
        Perm(PlatformPermissions.TenantsRead, "Visualizar tenants da plataforma."),
        Perm(PlatformPermissions.TenantsManage, "Gerenciar status de tenants da plataforma."),
        Perm(PlatformPermissions.UsersRead, "Visualizar usuários cross-tenant."),
        Perm(PlatformPermissions.UsersManage, "Gerenciar usuários cross-tenant."),
        Perm(PlatformPermissions.ReportsRead, "Visualizar relatórios administrativos."),
        Perm(PlatformPermissions.SupportAccess, "Acessar modo suporte assistido."),
        Perm(PlatformPermissions.AuditRead, "Visualizar auditoria administrativa."),
    ];

    private static object Perm(string code, string description) => new
    {
        Id = PermissionIds[code],
        Code = code,
        Description = description
    };

    private static object[] GetRoles() =>
    [
        Role(RolePlatformAdminId, Roles.PlatformAdmin, "Administrador global da plataforma."),
        Role(RoleTenantOwnerId, Roles.TenantOwner, "Dono da empresa/agência."),
        Role(RoleAgencyAdminId, Roles.AgencyAdmin, "Administrador da agência."),
        Role(RoleConsultantId, Roles.Consultant, "Consultor de viagens."),
        Role(RoleFinancialId, Roles.Financial, "Usuário financeiro."),
        Role(RoleOperatorId, Roles.Operator, "Operador de emissão e operação."),
        Role(RoleCustomerViewerId, Roles.CustomerViewer, "Usuário com acesso somente leitura."),
    ];

    private static object Role(Guid id, string name, string description) => new
    {
        Id = id,
        TenantId = PlatformTenantId,
        Name = name,
        Description = description,
        IsSystemRole = true,
        CreatedAtUtc = SeedDate,
        CreatedBy = "system",
        IsDeleted = false
    };

    private static object[] GetRolePermissions()
    {
        var map = new Dictionary<Guid, string[]>
        {
            [RolePlatformAdminId] = Permissions.All.Concat(PlatformPermissions.All).ToArray(),
            [RoleTenantOwnerId] =
            [
                Permissions.UsersRead, Permissions.UsersManage,
                Permissions.SettingsRead, Permissions.SettingsManage,
                Permissions.CustomersRead, Permissions.CustomersWrite,
                Permissions.TravelersRead, Permissions.TravelersWrite,
                Permissions.QuotesRead, Permissions.QuotesWrite, Permissions.QuotesApprove,
                Permissions.ProposalsRead, Permissions.ProposalsWrite, Permissions.ProposalsSend,
                Permissions.PaymentsRead, Permissions.PaymentsWrite,
                Permissions.MilesRead, Permissions.MilesWrite,
                Permissions.ReportsRead
            ],
            [RoleAgencyAdminId] =
            [
                Permissions.UsersRead, Permissions.UsersManage,
                Permissions.SettingsRead, Permissions.SettingsManage,
                Permissions.CustomersRead, Permissions.CustomersWrite,
                Permissions.TravelersRead, Permissions.TravelersWrite,
                Permissions.QuotesRead, Permissions.QuotesWrite, Permissions.QuotesApprove,
                Permissions.ProposalsRead, Permissions.ProposalsWrite, Permissions.ProposalsSend,
                Permissions.PaymentsRead, Permissions.PaymentsWrite,
                Permissions.MilesRead, Permissions.MilesWrite,
                Permissions.ReportsRead
            ],
            [RoleConsultantId] =
            [
                Permissions.CustomersRead, Permissions.CustomersWrite,
                Permissions.TravelersRead, Permissions.TravelersWrite,
                Permissions.QuotesRead, Permissions.QuotesWrite,
                Permissions.ProposalsRead, Permissions.ProposalsWrite, Permissions.ProposalsSend,
                Permissions.MilesRead
            ],
            [RoleFinancialId] =
            [
                Permissions.CustomersRead,
                Permissions.QuotesRead, Permissions.ProposalsRead,
                Permissions.PaymentsRead, Permissions.PaymentsWrite,
                Permissions.ReportsRead
            ],
            [RoleOperatorId] =
            [
                Permissions.CustomersRead, Permissions.TravelersRead,
                Permissions.QuotesRead, Permissions.QuotesWrite,
                Permissions.ProposalsRead,
                Permissions.PaymentsRead,
                Permissions.MilesRead, Permissions.MilesWrite
            ],
            [RoleCustomerViewerId] =
            [
                Permissions.CustomersRead, Permissions.TravelersRead,
                Permissions.QuotesRead, Permissions.ProposalsRead,
                Permissions.PaymentsRead, Permissions.MilesRead
            ],
        };

        return map
            .SelectMany(kv => kv.Value.Select(code => (object)new { RoleId = kv.Key, PermissionId = PermissionIds[code] }))
            .ToArray();
    }

    private static object[] GetPaymentProviders() =>
    [
        Provider(Guid.Parse("55555555-5555-5555-5555-555555550001"), "manual", "Manual"),
        Provider(Guid.Parse("55555555-5555-5555-5555-555555550002"), "asaas", "Asaas"),
        Provider(Guid.Parse("55555555-5555-5555-5555-555555550003"), "pagarme", "Pagar.me"),
        Provider(Guid.Parse("55555555-5555-5555-5555-555555550004"), "infinitepay", "InfinitePay"),
        Provider(Guid.Parse("55555555-5555-5555-5555-555555550005"), "mercadopago", "Mercado Pago"),
        Provider(Guid.Parse("55555555-5555-5555-5555-555555550006"), "stripe", "Stripe"),
    ];

    private static object Provider(Guid id, string code, string name) => new
    {
        Id = id,
        Code = code,
        Name = name,
        Status = PaymentProviderStatus.Active
    };

    private static object[] GetSubscriptionPlans() =>
    [
        Plan(StarterPlanId, "Starter", "Plano inicial para consultores individuais.", 49.90m, 499.00m, 1, 30),
        Plan(AgencyPlanId, "Agency", "Plano para pequenas agências.", 149.90m, 1499.00m, 5, 200),
        Plan(ProfessionalPlanId, "Professional", "Plano profissional para operações maiores.", 299.90m, 2999.00m, 15, 1000),
        Plan(EnterprisePlanId, "Enterprise", "Plano enterprise com limites customizados.", null, null, null, null),
    ];

    private static object Plan(Guid id, string name, string description, decimal? monthly, decimal? annual, int? maxUsers, int? maxQuotes) => new
    {
        Id = id,
        Name = name,
        Description = description,
        MonthlyPrice = monthly,
        AnnualPrice = annual,
        MaxUsers = maxUsers,
        MaxQuotesPerMonth = maxQuotes,
        Status = SubscriptionPlanStatus.Active,
        CreatedAtUtc = SeedDate,
        CreatedBy = "system",
        IsDeleted = false
    };

    private static object[] GetPlanFeatures() =>
    [
        // Starter
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880001"), StarterPlanId, "crm", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880002"), StarterPlanId, "quotes", 30),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880003"), StarterPlanId, "pricing", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880004"), StarterPlanId, "proposal_pdf", null),
        // Agency
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880005"), AgencyPlanId, "crm", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880006"), AgencyPlanId, "quotes", 200),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880007"), AgencyPlanId, "pricing", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880008"), AgencyPlanId, "proposal_pdf", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880009"), AgencyPlanId, "customer_portal", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880010"), AgencyPlanId, "payments", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880011"), AgencyPlanId, "branding", null),
        // Professional
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880012"), ProfessionalPlanId, "crm", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880013"), ProfessionalPlanId, "quotes", 1000),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880014"), ProfessionalPlanId, "pricing", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880015"), ProfessionalPlanId, "proposal_pdf", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880016"), ProfessionalPlanId, "customer_portal", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880017"), ProfessionalPlanId, "payments", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880018"), ProfessionalPlanId, "miles", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880019"), ProfessionalPlanId, "reports", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880020"), ProfessionalPlanId, "branding", null),
        // Enterprise
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880021"), EnterprisePlanId, "crm", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880022"), EnterprisePlanId, "quotes", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880023"), EnterprisePlanId, "pricing", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880024"), EnterprisePlanId, "proposal_pdf", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880025"), EnterprisePlanId, "customer_portal", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880026"), EnterprisePlanId, "payments", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880027"), EnterprisePlanId, "miles", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880028"), EnterprisePlanId, "reports", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880029"), EnterprisePlanId, "branding", null),
        Feature(Guid.Parse("88888888-8888-8888-8888-888888880030"), EnterprisePlanId, "white_label", null),
    ];

    private static object Feature(Guid id, Guid planId, string code, int? limit) => new
    {
        Id = id,
        SubscriptionPlanId = planId,
        FeatureCode = code,
        LimitValue = limit,
        Enabled = true
    };

    private static object[] GetLoyaltyPrograms() =>
    [
        Loyalty(Guid.Parse("66666666-6666-6666-6666-666666660001"), "Smiles", "Brazil", "GOL"),
        Loyalty(Guid.Parse("66666666-6666-6666-6666-666666660002"), "LATAM Pass", "Brazil", "LATAM"),
        Loyalty(Guid.Parse("66666666-6666-6666-6666-666666660003"), "TudoAzul", "Brazil", "Azul"),
        Loyalty(Guid.Parse("66666666-6666-6666-6666-666666660004"), "Livelo", "Brazil", null),
        Loyalty(Guid.Parse("66666666-6666-6666-6666-666666660005"), "Esfera", "Brazil", null),
        Loyalty(Guid.Parse("66666666-6666-6666-6666-666666660006"), "TAP Miles&Go", "Portugal", "TAP"),
        Loyalty(Guid.Parse("66666666-6666-6666-6666-666666660007"), "AAdvantage", "United States", "American Airlines"),
        Loyalty(Guid.Parse("66666666-6666-6666-6666-666666660008"), "Aeroplan", "Canada", "Air Canada"),
        Loyalty(Guid.Parse("66666666-6666-6666-6666-666666660009"), "Flying Blue", "France/Netherlands", "Air France-KLM"),
    ];

    private static object Loyalty(Guid id, string name, string country, string? airline) => new
    {
        Id = id,
        Name = name,
        Country = country,
        AirlineName = airline,
        Status = LoyaltyProgramStatus.Active,
        CreatedAtUtc = SeedDate,
        CreatedBy = "system",
        IsDeleted = false
    };

    private static object[] GetPaymentFeeRules() =>
    [
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770001"), PaymentMethod.CreditCard, 1, 4.20m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770002"), PaymentMethod.CreditCard, 2, 6.09m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770003"), PaymentMethod.CreditCard, 3, 7.01m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770004"), PaymentMethod.CreditCard, 4, 7.91m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770005"), PaymentMethod.CreditCard, 5, 8.80m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770006"), PaymentMethod.CreditCard, 6, 9.67m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770007"), PaymentMethod.CreditCard, 7, 12.59m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770008"), PaymentMethod.CreditCard, 8, 13.42m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770009"), PaymentMethod.CreditCard, 9, 14.25m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770010"), PaymentMethod.CreditCard, 10, 15.06m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770011"), PaymentMethod.CreditCard, 11, 15.87m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770012"), PaymentMethod.CreditCard, 12, 16.66m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770013"), PaymentMethod.Pix, 1, 0.00m),
        Fee(Guid.Parse("77777777-7777-7777-7777-777777770014"), PaymentMethod.Manual, 1, 0.00m),
    ];

    private static object Fee(Guid id, PaymentMethod method, int installments, decimal fee) => new
    {
        Id = id,
        TenantId = PlatformTenantId,
        PaymentMethod = method,
        Installments = installments,
        FeePercentage = fee,
        IsActive = true,
        CreatedAtUtc = SeedDate,
        CreatedBy = "system",
        IsDeleted = false
    };

    public static class Roles
    {
        public const string PlatformAdmin = "PlatformAdmin";
        public const string TenantOwner = "TenantOwner";
        public const string AgencyAdmin = "AgencyAdmin";
        public const string Consultant = "Consultant";
        public const string Financial = "Financial";
        public const string Operator = "Operator";
        public const string CustomerViewer = "CustomerViewer";
    }

    public static class Permissions
    {
        public const string TenantsRead = "tenants.read";
        public const string TenantsWrite = "tenants.write";
        public const string UsersRead = "users.read";
        public const string UsersManage = "users.manage";
        public const string SettingsRead = "settings.read";
        public const string SettingsManage = "settings.manage";
        public const string CustomersRead = "customers.read";
        public const string CustomersWrite = "customers.write";
        public const string TravelersRead = "travelers.read";
        public const string TravelersWrite = "travelers.write";
        public const string QuotesRead = "quotes.read";
        public const string QuotesWrite = "quotes.write";
        public const string QuotesApprove = "quotes.approve";
        public const string ProposalsRead = "proposals.read";
        public const string ProposalsWrite = "proposals.write";
        public const string ProposalsSend = "proposals.send";
        public const string PaymentsRead = "payments.read";
        public const string PaymentsWrite = "payments.write";
        public const string MilesRead = "miles.read";
        public const string MilesWrite = "miles.write";
        public const string ReportsRead = "reports.read";
        public const string AuditRead = "audit.read";

        public static readonly string[] All =
        [
            TenantsRead, TenantsWrite, UsersRead, UsersManage,
            SettingsRead, SettingsManage, CustomersRead, CustomersWrite,
            TravelersRead, TravelersWrite, QuotesRead, QuotesWrite, QuotesApprove,
            ProposalsRead, ProposalsWrite, ProposalsSend,
            PaymentsRead, PaymentsWrite, MilesRead, MilesWrite,
            ReportsRead, AuditRead
        ];
    }

    public static class PlatformPermissions
    {
        public const string DashboardRead = "platform.dashboard.read";
        public const string TenantsRead = "platform.tenants.read";
        public const string TenantsManage = "platform.tenants.manage";
        public const string UsersRead = "platform.users.read";
        public const string UsersManage = "platform.users.manage";
        public const string ReportsRead = "platform.reports.read";
        public const string SupportAccess = "platform.support.access";
        public const string AuditRead = "platform.audit.read";

        public static readonly string[] All =
        [
            DashboardRead, TenantsRead, TenantsManage,
            UsersRead, UsersManage, ReportsRead,
            SupportAccess, AuditRead
        ];
    }
}
