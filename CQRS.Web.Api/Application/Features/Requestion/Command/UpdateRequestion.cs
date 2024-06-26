using AutoWrapper.Wrappers;
using CQRS.Web.Api.Application.Common.Interface;
using CQRS.Web.Api.Domain.Enums;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Web.Api.Application.Features.Requestion.Command
{
    public class UpdateRequestion
    {
        public class Command : IRequest<ApiResponse>
        {
            public Guid Id { get; set; }
            public Status Status { get; set; }
            public long UserId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty().WithMessage("RequestionId cannot be null");
                RuleFor(x => x.Status).NotEmpty().WithMessage("Status cannot be null");
            }
        }

        public class Handler : IRequestHandler<Command, ApiResponse>
        {
            private readonly IApplicationDbContext _context;
            private readonly IUserServiceHelper _userServices;
            public Handler(IApplicationDbContext context, IUserServiceHelper userServices)
            {
                _context = context;
                _userServices = userServices;
            }

            public async Task<ApiResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var currentUser = await _userServices.CheckCurrentUser(request.UserId, cancellationToken);

                    var existingRequestion = await _context.Requestions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                    if (existingRequestion == null)
                        return new ApiResponse("Requestion Not Found", statusCode: 404);

                    existingRequestion.Status = request.Status;
                    existingRequestion.UpdatedAt = DateTime.UtcNow;
                    existingRequestion.UpdatedBy = currentUser.Id;
                    _context.Requestions.Update(existingRequestion);

                    await _context.SaveChangesAsync(cancellationToken);

                    return new ApiResponse("Requestion status has been Updated");

                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }
    }

}
