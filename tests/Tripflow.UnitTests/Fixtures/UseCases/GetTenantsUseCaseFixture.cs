using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Tripflow.Application.UseCases.Tenants;
using Tripflow.Domain.Common;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class GetTenantsUseCaseFixture : IDisposable
{
    public IMapper Mapper { get; }
    public Mock<ITenantRepository> MockRepository { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public GetTenantsUseCase UseCase { get; private set; } = null!;

    public GetTenantsUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    private void ResetMocks()
    {
        MockRepository = new Mock<ITenantRepository>();
        MockUserContext = new Mock<IUserContext>();
        MockUserPermissionService = new Mock<IUserPermissionService>();
        MockUserContext.Setup(u => u.IsAuthenticated).Returns(true);
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        UseCase = new GetTenantsUseCase(
            MockRepository.Object,
            MockUserContext.Object,
            MockUserPermissionService.Object,
            Mapper);
    }

    public GetTenantsUseCase Create(bool withData)
    {
        ResetMocks();
        var tenants = withData
            ? new List<Tenant> { TenantTestHelper.Create() }
            : new List<Tenant>();

        var totalItems = tenants.Count;
        var totalPages = totalItems > 0 ? 1 : 0;

        MockRepository
            .Setup(repo => repo.GetPagedAsync(
                It.IsAny<Expression<Func<Tenant, bool>>?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<Tenant, object>>?>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<Tenant, object>>[]>()))
            .ReturnsAsync(new PagedResult<Tenant>
            {
                Items = tenants,
                Page = 1,
                PageSize = 10,
                TotalItems = totalItems,
                TotalPages = totalPages
            });

        return UseCase;
    }

    public GetTenantsUseCase CreateWithCustomPagedResult(
        IEnumerable<Tenant> items,
        int page = 1,
        int pageSize = 10,
        int totalItems = 0,
        int totalPages = 0)
    {
        ResetMocks();
        var itemsList = items.ToList();
        var total = totalItems > 0 ? totalItems : itemsList.Count;
        var pages = totalPages > 0 ? totalPages : (total > 0 ? (int)Math.Ceiling(total / (double)pageSize) : 0);

        MockRepository
            .Setup(repo => repo.GetPagedAsync(
                It.IsAny<Expression<Func<Tenant, bool>>?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<Tenant, object>>?>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<Tenant, object>>[]>()))
            .ReturnsAsync(new PagedResult<Tenant>
            {
                Items = itemsList,
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = pages
            });

        return UseCase;
    }

    public void Dispose() { }
}
