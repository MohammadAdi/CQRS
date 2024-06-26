using AutoWrapper.Wrappers;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Web.Api.Features.User.Commands
{
    public class DeleteUser
    {
        public class Command : IRequest<ApiResponse>
        {
            public long Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty().WithMessage("Id cannot be null");
            }
        }

        public class Handler : IRequestHandler<Command, ApiResponse>
        {
            private readonly IApplicationDbContext _context;
            public Handler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
                    if (existingUser is null)
                        return new ApiResponse("User Not Found", statusCode: 404);

                    existingUser.isActive = false;
                    existingUser.UpdatedAt = DateTime.UtcNow;
                    _context.Users.Update(existingUser);
                    await _context.SaveChangesAsync(cancellationToken);
                    return new ApiResponse("User has been deleted", request.Id);
                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }

    }
}
