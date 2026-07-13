using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Application.Builders;

public static class QuoteFilterExtensions
{
    public static Expression<Func<Quote, bool>> ToExpression(this QuoteFilterRequest request)
    {
        Expression<Func<Quote, bool>> filter = x => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            filter = filter.And(x =>
                x.QuoteNumber.ToLower().Contains(search) ||
                x.Title.ToLower().Contains(search) ||
                (x.Origin != null && x.Origin.ToLower().Contains(search)) ||
                (x.Destination != null && x.Destination.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim().ToLower();
            filter = filter.And(x => x.Title.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.QuoteNumber))
        {
            var number = request.QuoteNumber.Trim().ToLower();
            filter = filter.And(x => x.QuoteNumber.ToLower().Contains(number));
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            var title = request.Title.Trim().ToLower();
            filter = filter.And(x => x.Title.ToLower().Contains(title));
        }

        if (!string.IsNullOrWhiteSpace(request.Origin))
        {
            var origin = request.Origin.Trim().ToLower();
            filter = filter.And(x => x.Origin != null && x.Origin.ToLower().Contains(origin));
        }

        if (!string.IsNullOrWhiteSpace(request.Destination))
        {
            var destination = request.Destination.Trim().ToLower();
            filter = filter.And(x => x.Destination != null && x.Destination.ToLower().Contains(destination));
        }

        if (request.CustomerId.HasValue)
        {
            var customerId = request.CustomerId.Value;
            filter = filter.And(x => x.CustomerId == customerId);
        }

        if (request.Type.HasValue)
        {
            var type = request.Type.Value;
            filter = filter.And(x => x.Type == type);
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            filter = filter.And(x => x.Status == status);
        }

        if (request.DepartureFrom.HasValue)
        {
            var from = request.DepartureFrom.Value;
            filter = filter.And(x => x.DepartureDate.HasValue && x.DepartureDate.Value >= from);
        }

        if (request.DepartureTo.HasValue)
        {
            var to = request.DepartureTo.Value;
            filter = filter.And(x => x.DepartureDate.HasValue && x.DepartureDate.Value <= to);
        }

        if (request.ReturnFrom.HasValue)
        {
            var from = request.ReturnFrom.Value;
            filter = filter.And(x => x.ReturnDate.HasValue && x.ReturnDate.Value >= from);
        }

        if (request.ReturnTo.HasValue)
        {
            var to = request.ReturnTo.Value;
            filter = filter.And(x => x.ReturnDate.HasValue && x.ReturnDate.Value <= to);
        }

        return filter;
    }
}
