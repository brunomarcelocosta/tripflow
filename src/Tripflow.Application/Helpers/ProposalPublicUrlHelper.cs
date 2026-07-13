using Microsoft.Extensions.Options;
using Tripflow.Application.Options;

namespace Tripflow.Application.Helpers;

public static class ProposalPublicUrlHelper
{
    public static string BuildPublicUrl(string publicToken, FrontendOptions? options)
    {
        var baseUrl = options?.PublicProposalBaseUrl?.TrimEnd('/');
        if (string.IsNullOrWhiteSpace(baseUrl))
            return $"/public/proposals/{publicToken}";
        return $"{baseUrl}/{publicToken}";
    }
}
