using Tripflow.Domain.Entities.Audit;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Audit;

public class AuditLogRepository(TripflowDbContext context) : BaseRepository<AuditLog>(context), IAuditLogRepository
{
}
