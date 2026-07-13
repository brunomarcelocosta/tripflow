using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.Helpers;

public static class QuoteStatusGuard
{
    public static bool IsLocked(Quote quote)
        => quote.Status is QuoteStatus.Cancelled or QuoteStatus.Paid or QuoteStatus.Issued;
}
