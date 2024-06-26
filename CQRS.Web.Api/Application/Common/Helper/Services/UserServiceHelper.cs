using AutoWrapper.Wrappers;
using CQRS.Web.Api.Application.Common.Interface;
using CQRS.Web.Api.Domain.Entities;
using CQRS.Web.Api.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Web.Api.Application.Common.Helper.Services
{
    public class UserServiceHelper : IUserServiceHelper
    {
        private readonly IApplicationDbContext _context;

        public UserServiceHelper(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> CheckCurrentUser(long userId, CancellationToken cancellationToken)
        {
            try
            {
                var currentUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
                if (currentUser == null)
                    throw new ApiException("User not found!");

                return currentUser;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message);
            }
        }
    }
}
