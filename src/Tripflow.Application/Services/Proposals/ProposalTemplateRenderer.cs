using System.Text;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Services.Proposals;

public class ProposalTemplateRenderer(IProposalRepository proposalRepository) : IProposalTemplateRenderer
{
    public async Task<string> RenderHtmlAsync(Guid proposalId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var proposal = await proposalRepository.GetFullByIdAndTenantAsync(proposalId, tenantId, cancellationToken)
            ?? throw new InvalidOperationException("Proposta não encontrada.");

        var quote = proposal.Quote;
        var branding = proposal.Tenant.Branding;
        var commercial = proposal.Tenant.CommercialSettings;
        var pricing = proposal.QuotePricingOption;
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html><html><head><meta charset=\"utf-8\"/>");
        sb.AppendLine("<style>body{font-family:Arial,sans-serif;margin:24px;color:#222}");
        sb.AppendLine($"h1{{color:{branding?.PrimaryColor ?? "#333"}}}");
        sb.AppendLine("table{border-collapse:collapse;width:100%;margin:12px 0}");
        sb.AppendLine("th,td{border:1px solid #ddd;padding:8px;text-align:left}</style></head><body>");

        if (!string.IsNullOrWhiteSpace(branding?.LogoUrl))
            sb.AppendLine($"<img src=\"{branding.LogoUrl}\" alt=\"Logo\" style=\"max-height:64px\"/>");

        sb.AppendLine($"<h1>Proposta {proposal.ProposalNumber}</h1>");
        sb.AppendLine($"<p><strong>{proposal.Tenant.TradeName ?? proposal.Tenant.LegalName}</strong></p>");

        if (commercial is not null)
        {
            sb.AppendLine("<p>");
            if (!string.IsNullOrWhiteSpace(commercial.CommercialEmail)) sb.AppendLine($"Email: {commercial.CommercialEmail}<br/>");
            if (!string.IsNullOrWhiteSpace(commercial.CommercialPhone)) sb.AppendLine($"Telefone: {commercial.CommercialPhone}<br/>");
            if (!string.IsNullOrWhiteSpace(commercial.WhatsApp)) sb.AppendLine($"WhatsApp: {commercial.WhatsApp}<br/>");
            sb.AppendLine("</p>");
        }

        sb.AppendLine($"<h2>Cotação: {quote.Title} ({quote.QuoteNumber})</h2>");
        if (quote.Customer is not null)
            sb.AppendLine($"<p>Cliente: {quote.Customer.FullName}</p>");

        sb.AppendLine("<table><tr><th>Origem</th><th>Destino</th><th>Ida</th><th>Volta</th></tr>");
        sb.AppendLine($"<tr><td>{quote.Origin ?? "-"}</td><td>{quote.Destination ?? "-"}</td><td>{quote.DepartureDate?.ToString() ?? "-"}</td><td>{quote.ReturnDate?.ToString() ?? "-"}</td></tr></table>");

        if (quote.Itinerary is not null)
        {
            sb.AppendLine($"<h3>Roteiro: {quote.Itinerary.Name}</h3><ul>");
            foreach (var stop in quote.Itinerary.Stops.OrderBy(s => s.Sequence))
                sb.AppendLine($"<li>{stop.City}, {stop.Country} — {stop.Nights} noite(s)</li>");
            sb.AppendLine("</ul>");
        }

        if (quote.FlightItems.Count > 0)
        {
            sb.AppendLine("<h3>Voos</h3>");
            foreach (var flight in quote.FlightItems)
            {
                sb.AppendLine($"<p><strong>{flight.AirlineName ?? "Voo"}</strong></p><ul>");
                foreach (var seg in flight.Segments.OrderBy(s => s.Sequence))
                    sb.AppendLine($"<li>{seg.OriginAirport} → {seg.DestinationAirport} {seg.FlightNumber}</li>");
                sb.AppendLine("</ul>");
            }
        }

        if (quote.Items.Count > 0)
        {
            sb.AppendLine("<h3>Itens</h3><table><tr><th>Título</th><th>Tipo</th><th>Valor</th></tr>");
            foreach (var item in quote.Items)
                sb.AppendLine($"<tr><td>{item.Title}</td><td>{item.Type}</td><td>{item.SaleAmount:C}</td></tr>");
            sb.AppendLine("</table>");
        }

        if (pricing is not null)
        {
            sb.AppendLine($"<h3>Opção de preço: {pricing.Name}</h3>");
            sb.AppendLine($"<p>Valor à vista: {pricing.CreditCashAmount:C} | Pix: {pricing.PixAmount:C}</p>");
            if (pricing.PaymentConditions.Count > 0)
            {
                sb.AppendLine("<h4>Condições de pagamento</h4><table><tr><th>Método</th><th>Parcelas</th><th>Valor</th></tr>");
                foreach (var cond in pricing.PaymentConditions)
                    sb.AppendLine($"<tr><td>{cond.PaymentMethod}</td><td>{cond.Installments}</td><td>{cond.GrossAmount:C}</td></tr>");
                sb.AppendLine("</table>");
            }
        }

        if (!string.IsNullOrWhiteSpace(commercial?.DefaultImportantNotes))
            sb.AppendLine($"<h3>Observações</h3><p>{commercial.DefaultImportantNotes}</p>");

        if (!string.IsNullOrWhiteSpace(commercial?.DefaultTerms))
            sb.AppendLine($"<h3>Termos</h3><p>{commercial.DefaultTerms}</p>");

        if (!string.IsNullOrWhiteSpace(branding?.ProposalFooter))
            sb.AppendLine($"<footer><p>{branding.ProposalFooter}</p></footer>");

        sb.AppendLine("</body></html>");
        return sb.ToString();
    }
}
