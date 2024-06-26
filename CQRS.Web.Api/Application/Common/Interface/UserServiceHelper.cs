using CQRS.Web.Api.Domain.Entities;

namespace CQRS.Web.Api.Application.Common.Interface
{
    public interface IUserServiceHelper
    {
        Task<User> CheckCurrentUser(long userId, CancellationToken cancellationToken);
    }
}
