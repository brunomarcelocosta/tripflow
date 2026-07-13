using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.DTOs.Responses.Customers;
using Tripflow.Application.UseCases.Customers.Interfaces;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Customers;

public class CreateCustomerUseCase(
    ICustomerRepository repository,
    IValidator<CreateCustomerRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CreateCustomerUseCase> logger) : ICreateCustomerUseCase
{
    public string ClassName = nameof(CreateCustomerUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<CustomerResponse?>> ExecuteAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
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
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("{ClassName} | {Method} | Erro de validação | TenantId={TenantId}", ClassName, Method, tenantId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<CustomerResponse?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        if (!string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            var existsDoc = await repository.ExistsByDocumentNumberAsync(tenantId, request.DocumentNumber, cancellationToken);

            if (existsDoc)
                return Result<CustomerResponse?>.Failure("Já existe um cliente cadastrado com este documento.");
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existsEmail = await repository.ExistsByEmailAsync(tenantId, request.Email, cancellationToken);

            if (existsEmail)
                return Result<CustomerResponse?>.Failure("Já existe um cliente cadastrado com este e-mail.");
        }

        try
        {
            var customer = new Customer(
                tenantId,
                request.Type,
                request.FullName,
                request.DocumentNumber,
                request.Email,
                request.Phone,
                request.BirthDate,
                request.Notes,
                createdBy);

            await repository.AddAsync(customer, cancellationToken);

            logger.LogInformation("{ClassName} | {Method} | Cliente cadastrado com sucesso | {Id}", ClassName, Method, customer.Id);

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
                0,
                customer.CreatedAtUtc,
                customer.UpdatedAtUtc);

            return Result<CustomerResponse?>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro inesperado ao cadastrar cliente | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantId, ex.Message);

            return Result<CustomerResponse?>.Failure("Erro inesperado ao criar cliente.");
        }
    }
}
