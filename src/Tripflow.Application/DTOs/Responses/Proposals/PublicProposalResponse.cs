using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Proposals;

public sealed record PublicProposalResponse(
    PublicProposalBrandingResponse Branding,
    PublicProposalDataResponse Proposal,
    PublicProposalQuoteResponse Quote,
    PublicProposalCustomerResponse? Customer,
    PublicProposalItineraryResponse? Itinerary,
    PublicProposalPricingResponse? Pricing,
    IEnumerable<PublicProposalPaymentConditionResponse> PaymentConditions);

public sealed record PublicProposalBrandingResponse(
    string? LogoUrl,
    string? PrimaryColor,
    string? SecondaryColor,
    string? TextColor,
    string? ProposalFooter,
    string? CommercialName);

public sealed record PublicProposalDataResponse(
    Guid Id,
    string ProposalNumber,
    ProposalStatus Status,
    string? PdfUrl,
    DateTime? ExpiresAtUtc);

public sealed record PublicProposalQuoteResponse(
    Guid Id,
    string QuoteNumber,
    string Title,
    QuoteType Type,
    string? Origin,
    string? Destination,
    DateOnly? DepartureDate,
    DateOnly? ReturnDate,
    int Adults,
    int Children,
    int Infants,
    string? Notes);

public sealed record PublicProposalCustomerResponse(
    Guid Id,
    string? FullName,
    string? Email,
    string? Phone);

public sealed record PublicProposalItineraryResponse(
    string Name,
    int? TotalDays,
    int? TotalNights,
    IEnumerable<PublicProposalItineraryStopResponse> Stops);

public sealed record PublicProposalItineraryStopResponse(
    int Order,
    string? Location,
    string? Description,
    DateOnly? Date);

public sealed record PublicProposalPricingResponse(
    Guid Id,
    string Name,
    decimal? PixAmount,
    decimal? CreditCashAmount);

public sealed record PublicProposalPaymentConditionResponse(
    PaymentMethod PaymentMethod,
    int Installments,
    decimal GrossAmount,
    decimal? InstallmentAmount,
    decimal? EstimatedFeeAmount,
    decimal? EstimatedNetAmount);
