using AutoWrapper.Wrappers;
using CQRS.Web.Api.Application.Common.Interface;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CQRS.Web.Api.Application.Features.Product.Command
{
    public class UpdateProduct
    {
        public class Command : IRequest<ApiResponse>
        {

            public long ProductId { get; set; }
            public string ProductName { get; set; }
            public int Qty { get; set; }
            public decimal ProductPrice { get; set; }
            public bool IsDeleted { get; set; }
            public long UserId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product Id cannot be null");
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

                    var existingProduct = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);
                    if (existingProduct == null)
                        return new ApiResponse("ProductId is not found", (int)HttpStatusCode.NotFound);

                    existingProduct.ProductName = request.ProductName;
                    existingProduct.Qty = request.Qty;
                    existingProduct.ProductPrice = request.ProductPrice;
                    existingProduct.IsDeleted = request.IsDeleted;
                    existingProduct.UpdatedAt = DateTime.UtcNow;
                    existingProduct.UpdatedBy = currentUser.Id;

                    _context.Products.Update(existingProduct);
                    await _context.SaveChangesAsync(cancellationToken);
                    return new ApiResponse("Product has been updated", existingProduct);
                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }

    }
}
