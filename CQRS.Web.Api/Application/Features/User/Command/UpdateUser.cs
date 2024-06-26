using AutoWrapper.Wrappers;
using CQRS.Web.Api.Domain.Enums;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Web.Api.Features.User.Commands
{
    public class UpdateUser
    {
        public class Command : IRequest<ApiResponse>
        {
            public long Id { get; set; }
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
                RuleFor(x => x.Id).NotEmpty().WithMessage("Id cannot be null");
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
                    var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
                    if (existingUser is null)
                        return new ApiResponse("User Not Found", statusCode: 404);

                    existingUser.Name = request.Name;
                    existingUser.Address = request.Address;
                    existingUser.Gender = request.Gender;
                    existingUser.PlaceOfBirth = request.PlaceOfBirth;
                    existingUser.BirthOfDate = request.BirthOfDate;
                    existingUser.Email = request.Email;
                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    existingUser.UpdatedAt = DateTime.UtcNow;
                    _context.Users.Update(existingUser);
                    await _context.SaveChangesAsync(cancellationToken);
                    return new ApiResponse("User has been updated", new Domain.Entities.User
                    {
                        Id = existingUser.Id,
                        Name = existingUser.Name,
                        Address = existingUser.Address,
                        Gender = existingUser.Gender,
                        PlaceOfBirth = existingUser.PlaceOfBirth,
                        BirthOfDate = existingUser.BirthOfDate,
                        Email = existingUser.Email
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
