using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Infra.Services;

public sealed class TenantRoleProvisioningService(
    TripflowDbContext dbContext,
    IPermissionRepository permissionRepository) : ITenantRoleProvisioningService
{
    public async Task ProvisionDefaultRolesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var existingRoleNames = await dbContext.Roles
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);

        var existingSet = existingRoleNames.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var allPermissionCodes = DefaultTenantRoleDefinitions.Templates
            .SelectMany(x => x.PermissionCodes)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var permissions = await permissionRepository.GetByCodesAsync(allPermissionCodes, cancellationToken);

        var permissionByCode = permissions.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

        foreach (var template in DefaultTenantRoleDefinitions.Templates)
        {
            if (existingSet.Contains(template.Name))
                continue;

            var role = new Role(
                tenantId,
                template.Name,
                template.Description,
                isSystemRole: true,
                createdBy: "system");

            foreach (var code in template.PermissionCodes.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!permissionByCode.TryGetValue(code, out var permission))
                    continue;

                role.RolePermissions.Add(new RolePermission(role.Id, permission.Id));
            }

            await dbContext.Roles.AddAsync(role, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
