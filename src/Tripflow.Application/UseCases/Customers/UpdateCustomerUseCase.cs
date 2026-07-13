using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.DTOs.Responses.Customers;
using Tripflow.Application.UseCases.Customers.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Customers;

public class UpdateCustomerUseCase(
    ICustomerRepository repository,
    ITravelerRepository travelerRepository,
    IValidator<UpdateCustomerRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<UpdateCustomerUseCase> logger) : IUpdateCustomerUseCase
{
    public string ClassName = nameof(UpdateCustomerUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<CustomerResponse?>> ExecuteAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<CustomerResponse?>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.CustomersWrite,
                cancellationToken))
        {
            return Result<CustomerResponse?>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("{ClassName} | {Method} | Erro de validação | Id={Id}", ClassName, Method, id);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<CustomerResponse?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var customer = await repository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);

        if (customer is null)
            return Result<CustomerResponse?>.Failure("Cliente não encontrado.");

        if (!string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            var existsDoc = await repository.ExistsByDocumentNumberExceptIdAsync(
                tenantId,
                request.DocumentNumber,
                id,
                cancellationToken);

            if (existsDoc)
                return Result<CustomerResponse?>.Failure("Já existe um cliente cadastrado com este documento.");
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existsEmail = await repository.ExistsByEmailExceptIdAsync(
                tenantId,
                request.Email,
                id,
                cancellationToken);

            if (existsEmail)
                return Result<CustomerResponse?>.Failure("Já existe um cliente cadastrado com este e-mail.");
        }

        try
        {
            customer.Update(
                request.Type,
                request.FullName,
                request.DocumentNumber,
                request.Email,
                request.Phone,
                request.BirthDate,
                request.Notes,
                request.Status,
                updatedBy);

            await repository.UpdateAsync(customer, cancellationToken);

            logger.LogInformation("{ClassName} | {Method} | Cliente atualizado com sucesso | {Id}", ClassName, Method, id);

            var counts = await travelerRepository.CountByCustomersAsync(
                tenantId,
                new[] { customer.Id },
                cancellationToken);

            var travelersCount = counts.TryGetValue(customer.Id, out var count) ? count : 0;

            var response = new CustomerResponse(
                customer.Id,
                customer.TenantId,
                customer.Type,
                customer.FullName,
                customer.DocumentNumber,
                customer.Email,
                customer.Phone,
                customer.BirthDate,
                customer.Notes,
                customer.Status,
                travelersCount,
                customer.CreatedAtUtc,
                customer.UpdatedAtUtc);

            return Result<CustomerResponse?>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro inesperado ao atualizar cliente | Id={Id} | {Message}",
                ClassName, Method, id, ex.Message);

            return Result<CustomerResponse?>.Failure("Erro inesperado ao atualizar cliente.");
        }
    }
}
