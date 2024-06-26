using AutoWrapper.Wrappers;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Web.Api.Features.User.Queries
{
    public class GetUserById
    {
        public class Query : IRequest<ApiResponse>
        {
            public long Id { get; set; }
        }
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Id).NotEmpty().WithMessage("Id cannot be null");
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
                    var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
                    if (existingUser is null)
                        return new ApiResponse("User Not Found", statusCode: 404);

                    return new ApiResponse("Fetch data succeeded", new {
                        Id = existingUser.Id,
                        Name = existingUser.Name,
                        Address = existingUser.Address,
                        Gender = existingUser.Gender,
                        PlaceOfBirth = existingUser.PlaceOfBirth,
                        BirthOfDate = existingUser.BirthOfDate,
                        Email = existingUser.Email,
                        IsActive = existingUser.isActive
                    });
                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }
    }
}
