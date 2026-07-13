using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Travelers;
using Tripflow.Application.DTOs.Responses.Travelers;
using Tripflow.Application.UseCases.Travelers.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Travelers;

public class UpdateTravelerUseCase(
    ITravelerRepository travelerRepository,
    IValidator<UpdateTravelerRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IMapper mapper,
    ILogger<UpdateTravelerUseCase> logger) : IUpdateTravelerUseCase
{
    public string ClassName = nameof(UpdateTravelerUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<TravelerResponse?>> ExecuteAsync(Guid customerId, Guid travelerId, UpdateTravelerRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<TravelerResponse?>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TravelerResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.TravelersWrite,
                cancellationToken))
        {
            return Result<TravelerResponse?>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("{ClassName} | {Method} | Erro de validação | Id={Id}", ClassName, Method, travelerId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<TravelerResponse?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var traveler = await travelerRepository.GetTrackedByCustomerAndTenantAsync(
            travelerId,
            customerId,
            tenantId,
            cancellationToken);

        if (traveler is null)
            return Result<TravelerResponse?>.Failure("Viajante não encontrado.");

        if (!string.IsNullOrWhiteSpace(request.PassportNumber))
        {
            var existsPassport = await travelerRepository.ExistsPassportNumberExceptIdAsync(
                tenantId,
                request.PassportNumber,
                travelerId,
                cancellationToken);

            if (existsPassport)
                return Result<TravelerResponse?>.Failure("Já existe um viajante cadastrado com este número de passaporte.");
        }

        try
        {
            traveler.Update(
                request.FullName,
                request.BirthDate,
                request.Nationality,
                request.DocumentNumber,
                request.PassportNumber,
                request.PassportExpirationDate,
                request.Notes,
                updatedBy);

            await travelerRepository.UpdateAsync(traveler, cancellationToken);

            logger.LogInformation("{ClassName} | {Method} | Viajante atualizado com sucesso | {Id}", ClassName, Method, travelerId);

            var response = mapper.Map<TravelerResponse>(traveler);

            return Result<TravelerResponse?>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro inesperado ao atualizar viajante | Id={Id} | {Message}",
                ClassName, Method, travelerId, ex.Message);

            return Result<TravelerResponse?>.Failure("Erro inesperado ao atualizar viajante.");
        }
    }
}
