using Tripflow.Application.DTOs.Responses.Customers;
using Tripflow.Application.DTOs.Responses.Pricing;
using Tripflow.Application.DTOs.Responses.Roles;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Application.DTOs.Responses.Travelers;
using Tripflow.Application.DTOs.Responses.Users;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.AutoMapper;

public class DomainToResponseTests
{
    [Fact]
    public void Map_Should_Map_UserProfile_To_GetCurrentUserResponse()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var tenant = TenantTestHelper.Create(tradeName: "Agência X");
        var permission = IdentityTestHelper.CreatePermission("users.manage");
        var role = IdentityTestHelper.CreateRole(tenant.Id, "AgencyAdmin", permissions: permission);
        var profile = IdentityTestHelper.CreateUserProfile(tenant: tenant, email: "admin@agencia.com");
        IdentityTestHelper.AddRoleToUser(profile, role);

        var response = mapper.Map<GetCurrentUserResponse>(profile);

        Assert.Equal(profile.Id, response.UserProfileId);
        Assert.Equal("Agência X", response.TenantTradeName);
        Assert.Equal("admin@agencia.com", response.Email);
        Assert.Contains("AgencyAdmin", response.Roles);
        Assert.Contains("users.manage", response.Permissions);
    }

    [Fact]
    public void Map_Should_Map_UserProfile_To_InviteUserResponse_With_Context_Items()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var profile = IdentityTestHelper.CreateUserProfile(email: "convidado@test.com");

        var response = mapper.Map<InviteUserResponse>(
            profile,
            opt =>
            {
                opt.Items["RoleNames"] = new[] { "Consultant" };
                opt.Items["InviteEmailSent"] = true;
            });

        Assert.Equal(profile.Id, response.UserProfileId);
        Assert.Equal("convidado@test.com", response.Email);
        Assert.Contains("Consultant", response.Roles);
        Assert.True(response.InviteEmailSent);
    }

    [Fact]
    public void Map_Should_Map_Role_To_RoleResponse()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var role = IdentityTestHelper.CreateRole(
            Guid.NewGuid(),
            "Consultant",
            permissions:
            [
                IdentityTestHelper.CreatePermission("users.read"),
                IdentityTestHelper.CreatePermission("customers.read")
            ]);

        var response = mapper.Map<RoleResponse>(role);

        Assert.Equal(role.Id, response.RoleId);
        Assert.Equal("Consultant", response.Name);
        Assert.Contains("users.read", response.Permissions);
        Assert.Contains("customers.read", response.Permissions);
    }

    [Fact]
    public void Map_Should_Map_Tenant_To_TenantResponse()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var tenant = TenantTestHelper.Create(
            legalName: "Empresa LTDA",
            tradeName: "Empresa",
            documentNumber: "11222333000181",
            email: "empresa@test.com",
            phone: "11999998888");

        var response = mapper.Map<TenantResponse>(tenant);

        Assert.Equal(tenant.Id, response.Id);
        Assert.Equal(tenant.LegalName, response.LegalName);
        Assert.Equal(tenant.TradeName, response.TradeName);
        Assert.Equal(tenant.DocumentNumber, response.DocumentNumber);
        Assert.Equal(tenant.Email, response.Email);
        Assert.Equal(tenant.Phone, response.Phone);
        Assert.Equal(tenant.Status, response.Status);
        Assert.Equal(tenant.CreatedAtUtc, response.CreatedAtUtc);
        Assert.Equal(tenant.UpdatedAtUtc, response.UpdatedAtUtc);
    }

    [Fact]
    public void Map_Should_Map_TenantBranding_To_TenantBrandingResponse()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var tenantId = Guid.NewGuid();
        var branding = TenantBrandingTestHelper.Create(
            tenantId: tenantId,
            logoUrl: "https://cdn.tripflow.test/logo.png",
            primaryColor: "#FF0000",
            secondaryColor: "#00FF00",
            textColor: "#000000",
            proposalFooter: "Rodapé.");

        var response = mapper.Map<TenantBrandingResponse>(branding);

        Assert.Equal(branding.Id, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("https://cdn.tripflow.test/logo.png", response.LogoUrl);
        Assert.Equal("#FF0000", response.PrimaryColor);
        Assert.Equal("#00FF00", response.SecondaryColor);
        Assert.Equal("#000000", response.TextColor);
        Assert.Equal("Rodapé.", response.ProposalFooter);
        Assert.Equal(branding.CreatedAtUtc, response.CreatedAtUtc);
    }

    [Fact]
    public void Map_Should_Map_TenantBranding_To_UploadTenantLogoResponse()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var branding = TenantBrandingTestHelper.Create(
            logoUrl: "/uploads/tenant-logos/abc.png");

        var response = mapper.Map<UploadTenantLogoResponse>(branding);

        Assert.Equal(branding.TenantId, response.TenantId);
        Assert.Equal("/uploads/tenant-logos/abc.png", response.LogoUrl);
    }

    [Fact]
    public void Map_Should_Map_TenantCommercialSettings_To_TenantCommercialSettingsResponse()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var tenantId = Guid.NewGuid();
        var settings = TenantCommercialSettingsTestHelper.CreateWithCommercialData(
            tenantId: tenantId,
            commercialEmail: "contato@empresa.com",
            website: "https://empresa.com",
            defaultProposalExpirationHours: 72);

        var response = mapper.Map<TenantCommercialSettingsResponse>(settings);

        Assert.Equal(settings.Id, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("contato@empresa.com", response.CommercialEmail);
        Assert.Equal("https://empresa.com", response.Website);
        Assert.Equal(72, response.DefaultProposalExpirationHours);
    }

    [Fact]
    public void Map_Should_Map_TenantCommercialSettings_To_TenantFinancialSettingsResponse()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var tenantId = Guid.NewGuid();
        var settings = TenantCommercialSettingsTestHelper.CreateWithFinancialData(
            tenantId: tenantId,
            defaultProfitAmount: 150m,
            defaultPixDiscountPercentage: 7.5m);

        var response = mapper.Map<TenantFinancialSettingsResponse>(settings);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(150m, response.DefaultProfitAmount);
        Assert.Null(response.DefaultProfitPercentage);
        Assert.Equal(7.5m, response.DefaultPixDiscountPercentage);
    }

    [Fact]
    public void Map_Should_Map_PaymentFeeRule_To_PaymentFeeRuleResponse()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var tenantId = Guid.NewGuid();
        var rule = new PaymentFeeRule(
            tenantId,
            PaymentMethod.CreditCard,
            3,
            7.5m,
            true,
            "system");

        var response = mapper.Map<PaymentFeeRuleResponse>(rule);

        Assert.Equal(rule.Id, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(PaymentMethod.CreditCard, response.PaymentMethod);
        Assert.Equal(3, response.Installments);
        Assert.Equal(7.5m, response.FeePercentage);
        Assert.True(response.IsActive);
    }

    [Fact]
    public void Map_Should_Map_Customer_To_CustomerResponse()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var tenantId = Guid.NewGuid();
        var customer = CustomerTestHelper.Create(
            tenantId: tenantId,
            type: CustomerType.Company,
            fullName: "Empresa LTDA",
            documentNumber: "11222333000181",
            email: "empresa@test.com");

        var response = mapper.Map<CustomerResponse>(customer);

        Assert.Equal(customer.Id, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(CustomerType.Company, response.Type);
        Assert.Equal("Empresa LTDA", response.FullName);
        Assert.Equal("11222333000181", response.DocumentNumber);
        Assert.Equal("empresa@test.com", response.Email);
        Assert.Equal(CustomerStatus.Active, response.Status);
        Assert.Equal(0, response.TravelersCount);
        Assert.Equal(customer.CreatedAtUtc, response.CreatedAtUtc);
    }

    [Fact]
    public void Map_Should_Map_Traveler_To_TravelerResponse_With_PassportStatus()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var tenantId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var traveler = TravelerTestHelper.Create(
            tenantId: tenantId,
            customerId: customerId,
            fullName: "Carlos Silva",
            passportNumber: "BR999",
            passportExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(2));

        var response = mapper.Map<TravelerResponse>(traveler);

        Assert.Equal(traveler.Id, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal("Carlos Silva", response.FullName);
        Assert.Equal("BR999", response.PassportNumber);
        Assert.False(response.PassportExpired);
        Assert.True(response.PassportExpiringSoon);
    }

    [Fact]
    public void Map_Should_Map_Traveler_To_TravelerResponse_With_Expired_Passport()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var traveler = TravelerTestHelper.Create(
            passportExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1));

        var response = mapper.Map<TravelerResponse>(traveler);

        Assert.True(response.PassportExpired);
        Assert.False(response.PassportExpiringSoon);
    }

    [Fact]
    public void Map_Should_Map_CustomerPreference_To_CustomerPreferenceResponse()
    {
        var mapper = AutoMapperTestHelper.CreateMapper();
        var tenantId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var pref = CustomerPreferenceTestHelper.CreateWithData(
            tenantId: tenantId,
            customerId: customerId,
            preferredAirlines: "GOL",
            generalNotes: "VIP");

        var response = mapper.Map<CustomerPreferenceResponse>(pref);

        Assert.Equal(pref.Id, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal("GOL", response.PreferredAirlines);
        Assert.Equal("VIP", response.GeneralNotes);
    }
}
