using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Travelers;
using Tripflow.Application.DTOs.Responses.Travelers;
using Tripflow.Application.UseCases.Travelers.Interfaces;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Travelers;

public class CreateTravelerUseCase(
    ICustomerRepository customerRepository,
    ITravelerRepository travelerRepository,
    IValidator<CreateTravelerRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IMapper mapper,
    ILogger<CreateTravelerUseCase> logger) : ICreateTravelerUseCase
{
    public string ClassName = nameof(CreateTravelerUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<TravelerResponse?>> ExecuteAsync(Guid customerId, CreateTravelerRequest request, CancellationToken cancellationToken = default)
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
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("{ClassName} | {Method} | Erro de validação | TenantId={TenantId}", ClassName, Method, tenantId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<TravelerResponse?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);

        if (customer is null)
            return Result<TravelerResponse?>.Failure("Cliente não encontrado.");

        if (!string.IsNullOrWhiteSpace(request.PassportNumber))
        {
            var existsPassport = await travelerRepository.ExistsPassportNumberAsync(
                tenantId,
                request.PassportNumber,
                cancellationToken);

            if (existsPassport)
                return Result<TravelerResponse?>.Failure("Já existe um viajante cadastrado com este número de passaporte.");
        }

        try
        {
            var traveler = new Traveler(
                tenantId,
                customerId,
                request.FullName,
                request.BirthDate,
                request.Nationality,
                request.DocumentNumber,
                request.PassportNumber,
                request.PassportExpirationDate,
                request.Notes,
                createdBy);

            await travelerRepository.AddAsync(traveler, cancellationToken);

            logger.LogInformation("{ClassName} | {Method} | Viajante cadastrado com sucesso | {Id}", ClassName, Method, traveler.Id);

            var response = mapper.Map<TravelerResponse>(traveler);

            return Result<TravelerResponse?>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro inesperado ao cadastrar viajante | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantId, ex.Message);

            return Result<TravelerResponse?>.Failure("Erro inesperado ao criar viajante.");
        }
    }
}
