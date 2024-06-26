using AutoWrapper.Wrappers;
using CQRS.Web.Api.Application.Common.Interface;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;

namespace CQRS.Web.Api.Application.Features.Product.Command
{
    public class CreateProduct
    {
        public class Command : IRequest<ApiResponse>
        {
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public int Qty { get; set; }
            public decimal ProductPrice { get; set; }

            public long UserId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ProductCode).NotEmpty().WithMessage("Product Code cannot be null");
                RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product Name cannot be null");
                RuleFor(x => x.Qty).LessThanOrEqualTo(0).WithMessage("Qty cannot be less than or equal to zero (0)");
                RuleFor(x => x.ProductPrice).LessThanOrEqualTo(0).WithMessage("Product price cannot be less than or equal to zero (0)");
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

                    var model = new Domain.Entities.Product()
                    {
                        ProductName = request.ProductName,
                        ProductCode = request.ProductCode,
                        Qty = request.Qty,
                        ProductPrice = request.ProductPrice,
                        IsDeleted = false,
                        CreatedBy = currentUser.Id,
                    };

                    _context.Products.Add(model);
                    await _context.SaveChangesAsync(cancellationToken);
                    return new ApiResponse("Product has been created", model);
                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }
    }
}
