using AutoWrapper.Wrappers;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Web.Api.Features.User.Queries
{
    public class GetAllUsers
    {
        public class Query : IRequest<ApiResponse>
        {

        }
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {

            }
        }
        public class Handler : IRequestHandler<Query, ApiResponse>
        {
            private readonly IApplicationDbContext _context;
            public Handler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var users = await _context.Users.Where(x => x.isActive).Select(x=> new {
                        Id = x.Id,
                        Name = x.Name,
                        Address = x.Address,
                        Gender = x.Gender,
                        PlaceOfBirth = x.PlaceOfBirth,
                        BirthOfDate = x.BirthOfDate,
                        Email = x.Email,
                        IsActive=x.isActive
                    }).ToListAsync();
                    return new ApiResponse("Fetch data succeeded", users);
                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }


    }
}