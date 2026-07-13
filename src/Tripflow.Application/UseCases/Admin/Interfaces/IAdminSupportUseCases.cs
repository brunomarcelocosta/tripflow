using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Admin;

namespace Tripflow.Application.UseCases.Admin.Interfaces;

public interface ICreateSupportSessionUseCase
{
    Task<Result<SupportSessionResponse>> ExecuteAsync(CreateSupportSessionRequest request, CancellationToken cancellationToken = default);
}

public interface IGetCurrentSupportSessionUseCase
{
    Task<Result<SupportSessionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IEndCurrentSupportSessionUseCase
{
    Task<Result<SupportSessionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IGetSupportSessionsUseCase
{
    Task<Result<PagedResponse<SupportSessionResponse>>> ExecuteAsync(FilterRequest request, CancellationToken cancellationToken = default);
}
