using AutoWrapper.Wrappers;
using CQRS.Web.Api.Domain.Enums;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;

namespace CQRS.Web.Api.Features.User.Commands
{
    public class CreateUser
    {
        public class Command : IRequest<ApiResponse>
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public Gender Gender { get; set; }
            public string PlaceOfBirth { get; set; }
            public DateTime BirthOfDate { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be null");
                RuleFor(x => x.Address).NotEmpty().WithMessage("Address cannot be null");
                RuleFor(x => x.Gender).NotEmpty().WithMessage("Gender cannot be null");
                RuleFor(x => x.PlaceOfBirth).NotEmpty().WithMessage("Place of birth cannot be null");
                RuleFor(x => x.BirthOfDate).NotEmpty().WithMessage("Birth of Date cannot be null");
                RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be null");
                RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be null");
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
                    var model = new Domain.Entities.User()
                    {
                        Name = request.Name,
                        Address = request.Address,
                        Gender = request.Gender,
                        PlaceOfBirth = request.PlaceOfBirth,
                        BirthOfDate = request.BirthOfDate,
                        Email = request.Email,
                        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                        CreatedAt = DateTime.UtcNow,
                        isActive = true
                    };
                    _context.Users.Add(model);
                    await _context.SaveChangesAsync(cancellationToken);
                    return new ApiResponse("User has been created", model);
                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }

    }
}
