using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Miles;

public class LoyaltyProgramRepository(TripflowDbContext context) : BaseRepository<LoyaltyProgram>(context), ILoyaltyProgramRepository
{
    public async Task<LoyaltyProgram?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.LoyaltyPrograms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<LoyaltyProgram?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LoyaltyPrograms
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.LoyaltyPrograms
            .AsNoTracking()
            .AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameExceptIdAsync(string name, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LoyaltyPrograms
            .AsNoTracking()
            .AnyAsync(x => x.Name == name && x.Id != id, cancellationToken);
    }

    public async Task<bool> HasLinkedAccountsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerLoyaltyAccounts
            .AsNoTracking()
            .AnyAsync(x => x.LoyaltyProgramId == id, cancellationToken);
    }
}
