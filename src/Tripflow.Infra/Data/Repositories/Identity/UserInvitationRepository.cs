using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Identity;

public class UserInvitationRepository(TripflowDbContext context) : BaseRepository<UserInvitation>(context), IUserInvitationRepository
{
}
