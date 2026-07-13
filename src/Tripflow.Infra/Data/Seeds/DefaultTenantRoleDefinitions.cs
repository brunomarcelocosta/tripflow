namespace Tripflow.Infra.Data.Seeds;

public static class DefaultTenantRoleDefinitions
{
    public sealed record RoleTemplate(string Name, string Description, string[] PermissionCodes);

    public static IReadOnlyList<RoleTemplate> Templates { get; } =
    [
        new(
            TripflowDbSeedData.Roles.TenantOwner,
            "Dono da empresa/agência.",
            [
                TripflowDbSeedData.Permissions.UsersRead, TripflowDbSeedData.Permissions.UsersManage,
                TripflowDbSeedData.Permissions.SettingsRead, TripflowDbSeedData.Permissions.SettingsManage,
                TripflowDbSeedData.Permissions.CustomersRead, TripflowDbSeedData.Permissions.CustomersWrite,
                TripflowDbSeedData.Permissions.TravelersRead, TripflowDbSeedData.Permissions.TravelersWrite,
                TripflowDbSeedData.Permissions.QuotesRead, TripflowDbSeedData.Permissions.QuotesWrite,
                TripflowDbSeedData.Permissions.QuotesApprove,
                TripflowDbSeedData.Permissions.ProposalsRead, TripflowDbSeedData.Permissions.ProposalsWrite,
                TripflowDbSeedData.Permissions.ProposalsSend,
                TripflowDbSeedData.Permissions.PaymentsRead, TripflowDbSeedData.Permissions.PaymentsWrite,
                TripflowDbSeedData.Permissions.MilesRead, TripflowDbSeedData.Permissions.MilesWrite,
                TripflowDbSeedData.Permissions.ReportsRead
            ]),
        new(
            TripflowDbSeedData.Roles.AgencyAdmin,
            "Administrador da agência.",
            [
                TripflowDbSeedData.Permissions.UsersRead, TripflowDbSeedData.Permissions.UsersManage,
                TripflowDbSeedData.Permissions.SettingsRead, TripflowDbSeedData.Permissions.SettingsManage,
                TripflowDbSeedData.Permissions.CustomersRead, TripflowDbSeedData.Permissions.CustomersWrite,
                TripflowDbSeedData.Permissions.TravelersRead, TripflowDbSeedData.Permissions.TravelersWrite,
                TripflowDbSeedData.Permissions.QuotesRead, TripflowDbSeedData.Permissions.QuotesWrite,
                TripflowDbSeedData.Permissions.QuotesApprove,
                TripflowDbSeedData.Permissions.ProposalsRead, TripflowDbSeedData.Permissions.ProposalsWrite,
                TripflowDbSeedData.Permissions.ProposalsSend,
                TripflowDbSeedData.Permissions.PaymentsRead, TripflowDbSeedData.Permissions.PaymentsWrite,
                TripflowDbSeedData.Permissions.MilesRead, TripflowDbSeedData.Permissions.MilesWrite,
                TripflowDbSeedData.Permissions.ReportsRead
            ]),
        new(
            TripflowDbSeedData.Roles.Consultant,
            "Consultor de viagens.",
            [
                TripflowDbSeedData.Permissions.CustomersRead, TripflowDbSeedData.Permissions.CustomersWrite,
                TripflowDbSeedData.Permissions.TravelersRead, TripflowDbSeedData.Permissions.TravelersWrite,
                TripflowDbSeedData.Permissions.QuotesRead, TripflowDbSeedData.Permissions.QuotesWrite,
                TripflowDbSeedData.Permissions.ProposalsRead, TripflowDbSeedData.Permissions.ProposalsWrite,
                TripflowDbSeedData.Permissions.ProposalsSend,
                TripflowDbSeedData.Permissions.MilesRead
            ]),
        new(
            TripflowDbSeedData.Roles.Financial,
            "Usuário financeiro.",
            [
                TripflowDbSeedData.Permissions.CustomersRead,
                TripflowDbSeedData.Permissions.QuotesRead, TripflowDbSeedData.Permissions.ProposalsRead,
                TripflowDbSeedData.Permissions.PaymentsRead, TripflowDbSeedData.Permissions.PaymentsWrite,
                TripflowDbSeedData.Permissions.ReportsRead
            ]),
        new(
            TripflowDbSeedData.Roles.Operator,
            "Operador de emissão e operação.",
            [
                TripflowDbSeedData.Permissions.CustomersRead, TripflowDbSeedData.Permissions.TravelersRead,
                TripflowDbSeedData.Permissions.QuotesRead, TripflowDbSeedData.Permissions.QuotesWrite,
                TripflowDbSeedData.Permissions.ProposalsRead,
                TripflowDbSeedData.Permissions.PaymentsRead,
                TripflowDbSeedData.Permissions.MilesRead, TripflowDbSeedData.Permissions.MilesWrite
            ]),
        new(
            TripflowDbSeedData.Roles.CustomerViewer,
            "Usuário com acesso somente leitura.",
            [
                TripflowDbSeedData.Permissions.CustomersRead, TripflowDbSeedData.Permissions.TravelersRead,
                TripflowDbSeedData.Permissions.QuotesRead, TripflowDbSeedData.Permissions.ProposalsRead,
                TripflowDbSeedData.Permissions.PaymentsRead, TripflowDbSeedData.Permissions.MilesRead
            ])
    ];
}
