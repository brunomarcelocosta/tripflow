using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Application.Helpers;

public static class PublicProposalMapper
{
    public static PublicProposalResponse Map(Proposal proposal)
    {
        var quote = proposal.Quote;
        var branding = proposal.Tenant.Branding;
        var commercial = proposal.Tenant.CommercialSettings;
        var pricing = proposal.QuotePricingOption;

        return new PublicProposalResponse(
            new PublicProposalBrandingResponse(
                branding?.LogoUrl,
                branding?.PrimaryColor,
                branding?.SecondaryColor,
                branding?.TextColor,
                branding?.ProposalFooter,
                proposal.Tenant.TradeName),
            new PublicProposalDataResponse(
                proposal.Id,
                proposal.ProposalNumber,
                proposal.Status,
                proposal.PdfUrl,
                proposal.ExpiresAtUtc),
            new PublicProposalQuoteResponse(
                quote.Id,
                quote.QuoteNumber,
                quote.Title,
                quote.Type,
                quote.Origin,
                quote.Destination,
                quote.DepartureDate,
                quote.ReturnDate,
                quote.Adults,
                quote.Children,
                quote.Infants,
                quote.Notes),
            quote.Customer is null ? null : new PublicProposalCustomerResponse(
                quote.Customer.Id,
                quote.Customer.FullName,
                quote.Customer.Email,
                quote.Customer.Phone),
            quote.Itinerary is null ? null : new PublicProposalItineraryResponse(
                quote.Itinerary.Name,
                quote.Itinerary.TotalDays,
                quote.Itinerary.TotalNights,
                quote.Itinerary.Stops
                    .OrderBy(s => s.Sequence)
                    .Select(s => new PublicProposalItineraryStopResponse(
                        s.Sequence,
                        $"{s.City}, {s.Country}",
                        s.Notes,
                        null))),
            pricing is null ? null : new PublicProposalPricingResponse(
                pricing.Id,
                pricing.Name,
                pricing.PixAmount,
                pricing.CreditCashAmount),
            pricing?.PaymentConditions.Select(c => new PublicProposalPaymentConditionResponse(
                c.PaymentMethod,
                c.Installments,
                c.GrossAmount,
                c.InstallmentAmount,
                c.EstimatedFeeAmount,
                c.EstimatedNetAmount)) ?? []);
    }
}
