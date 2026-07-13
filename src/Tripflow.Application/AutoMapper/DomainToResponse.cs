using AutoMapper;
using Tripflow.Application.DTOs.Responses.Customers;
using Tripflow.Application.DTOs.Responses.Miles;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.DTOs.Responses.Pricing;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.DTOs.Responses.Roles;
using Tripflow.Application.DTOs.Responses.Subscriptions;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Application.DTOs.Responses.Travelers;
using Tripflow.Application.DTOs.Responses.Users;
using Tripflow.Domain.Entities;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Application.AutoMapper;

public class DomainToResponse : Profile
{
    public DomainToResponse()
    {
        CreateMap<Tenant, TenantResponse>()
            .ReverseMap();

        CreateMap<UserProfile, GetCurrentUserResponse>()
            .ForMember(d => d.UserProfileId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.TenantTradeName, o => o.MapFrom(s => s.Tenant.TradeName))
            .ForMember(d => d.Entitlements, o => o.Ignore())
            .ForMember(d => d.Roles, o => o.MapFrom(s =>
                s.UserRoles.Select(ur => ur.Role.Name).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()))
            .ForMember(d => d.Permissions, o => o.MapFrom(s =>
                s.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Code)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(code => code)
                    .ToArray()));

        CreateMap<UserProfile, InviteUserResponse>()
            .ForMember(d => d.UserProfileId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Roles, o => o.MapFrom((_, _, _, ctx) =>
                ctx.Items.TryGetValue("RoleNames", out var names) && names is IEnumerable<string> roleNames
                    ? roleNames.ToArray()
                    : Array.Empty<string>()))
            .ForMember(d => d.InviteEmailSent, o => o.MapFrom((_, _, _, ctx) =>
                ctx.Items.TryGetValue("InviteEmailSent", out var sent) && sent is bool inviteSent && inviteSent));

        CreateMap<Role, RoleResponse>()
            .ForMember(d => d.RoleId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Permissions, o => o.MapFrom(s =>
                s.RolePermissions
                    .Select(rp => rp.Permission.Code)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(code => code)
                    .ToArray()));

        CreateMap<Role, string>().ConvertUsing(r => r.Name);

        CreateMap<TenantBranding, TenantBrandingResponse>();

        CreateMap<TenantBranding, UploadTenantLogoResponse>()
            .ForMember(d => d.LogoUrl, o => o.MapFrom(s => s.LogoUrl ?? string.Empty));

        CreateMap<TenantCommercialSettings, TenantCommercialSettingsResponse>();

        CreateMap<TenantCommercialSettings, TenantFinancialSettingsResponse>();

        CreateMap<PaymentFeeRule, PaymentFeeRuleResponse>();

        CreateMap<Customer, CustomerResponse>()
            .ForCtorParam(nameof(CustomerResponse.TravelersCount), o => o.MapFrom(s => s.Travelers != null ? s.Travelers.Count : 0));

        CreateMap<Traveler, TravelerResponse>()
            .ForCtorParam(nameof(TravelerResponse.PassportExpired), o => o.MapFrom(s => s.IsPassportExpired()))
            .ForCtorParam(nameof(TravelerResponse.PassportExpiringSoon), o => o.MapFrom(s => s.IsPassportExpiringSoon(6)));

        CreateMap<CustomerPreference, CustomerPreferenceResponse>();

        CreateMap<QuoteItem, QuoteItemResponse>()
            .ForCtorParam(nameof(QuoteItemResponse.EstimatedProfitAmount), o => o.MapFrom(s => s.GetEstimatedProfitAmount()));

        CreateMap<ItineraryStop, ItineraryStopResponse>();

        CreateMap<Itinerary, ItineraryResponse>()
            .ForCtorParam(nameof(ItineraryResponse.Stops), o => o.MapFrom(s => s.Stops.OrderBy(st => st.Sequence)));

        CreateMap<FlightSegment, FlightSegmentResponse>();

        CreateMap<QuoteFlightItem, QuoteFlightItemResponse>()
            .ForCtorParam(nameof(QuoteFlightItemResponse.Segments), o => o.MapFrom(s => s.Segments.OrderBy(seg => seg.Sequence)));

        CreateMap<QuotePaymentCondition, QuotePaymentConditionResponse>();

        CreateMap<QuotePricingOption, QuotePricingOptionResponse>();

        CreateMap<MilesQuoteOption, MilesQuoteOptionResponse>()
            .ForCtorParam(nameof(MilesQuoteOptionResponse.LoyaltyProgramName), o => o.MapFrom(s => s.LoyaltyProgram != null ? s.LoyaltyProgram.Name : null));

        CreateMap<LoyaltyProgram, LoyaltyProgramResponse>();

        CreateMap<CustomerLoyaltyAccount, CustomerLoyaltyAccountResponse>()
            .ForCtorParam(nameof(CustomerLoyaltyAccountResponse.CustomerName), o => o.MapFrom(s => s.Customer.FullName))
            .ForCtorParam(nameof(CustomerLoyaltyAccountResponse.LoyaltyProgramName), o => o.MapFrom(s => s.LoyaltyProgram.Name))
            .ForCtorParam(nameof(CustomerLoyaltyAccountResponse.ExpiringMilesInNext90Days), o => o.MapFrom(_ => 0));

        CreateMap<MilesTransaction, MilesTransactionResponse>();

        CreateMap<MilesExpirationBatch, MilesExpirationBatchResponse>();

        CreateMap<Proposal, ProposalResponse>()
            .ForCtorParam(nameof(ProposalResponse.VersionsCount), o => o.MapFrom(s => s.Versions.Count))
            .ForCtorParam(nameof(ProposalResponse.EventsCount), o => o.MapFrom(s => s.Events.Count));

        CreateMap<ProposalVersion, ProposalVersionResponse>();

        CreateMap<ProposalEvent, ProposalEventResponse>();

        CreateMap<PaymentProvider, PaymentProviderResponse>();

        CreateMap<TenantPaymentProvider, TenantPaymentProviderResponse>()
            .ForCtorParam(nameof(TenantPaymentProviderResponse.PaymentProviderCode), o => o.MapFrom(s => s.PaymentProvider.Code))
            .ForCtorParam(nameof(TenantPaymentProviderResponse.PaymentProviderName), o => o.MapFrom(s => s.PaymentProvider.Name))
            .ForCtorParam(nameof(TenantPaymentProviderResponse.HasApiKey), o => o.MapFrom(s => !string.IsNullOrEmpty(s.EncryptedApiKey)))
            .ForCtorParam(nameof(TenantPaymentProviderResponse.HasSecret), o => o.MapFrom(s => !string.IsNullOrEmpty(s.EncryptedSecret)))
            .ForCtorParam(nameof(TenantPaymentProviderResponse.HasWebhookSecret), o => o.MapFrom(s => !string.IsNullOrEmpty(s.WebhookSecret)));

        CreateMap<Payment, PaymentResponse>()
            .ForCtorParam(nameof(PaymentResponse.Links), o => o.MapFrom(s => s.Links));

        CreateMap<PaymentLink, PaymentLinkResponse>();

        CreateMap<FinancialTransaction, FinancialTransactionResponse>();

        CreateMap<PlanFeature, PlanFeatureResponse>();

        CreateMap<SubscriptionPlan, SubscriptionPlanResponse>()
            .ForMember(d => d.Features, o => o.MapFrom(s => s.Features));

        CreateMap<TenantSubscription, TenantSubscriptionResponse>()
            .ForMember(d => d.SubscriptionPlanName, o => o.MapFrom(s => s.SubscriptionPlan != null ? s.SubscriptionPlan.Name : string.Empty));

        CreateMap<TenantUsage, TenantUsageResponse>();

        CreateMap<Lead, LeadResponse>();

        CreateMap<DateTime, DateOnly>().ConvertUsing(dt => DateOnly.FromDateTime(dt));
        CreateMap<DateTime?, DateOnly?>().ConvertUsing(dt => dt.HasValue ? DateOnly.FromDateTime(dt.Value) : null);
        CreateMap<DateOnly, DateTime>().ConvertUsing(dt => dt.ToDateTime(new TimeOnly()));
        CreateMap<DateOnly?, DateTime?>().ConvertUsing(dt => dt.HasValue ? dt.Value.ToDateTime(new TimeOnly()) : null);
    }
}
