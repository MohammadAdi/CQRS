using AutoWrapper.Wrappers;
using CQRS.Web.Api.Application.Common.Interface;
using CQRS.Web.Api.Application.Features.Product.Model;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CQRS.Web.Api.Application.Features.Product.Query
{
    public class GetProductById
    {
        public class Query : IRequest<ApiResponse>
        {
            public int ProductId { get; set; }
        }
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.ProductId).NotEmpty().WithMessage("Id cannot be null");
            }
        }
        public class Handler : IRequestHandler<Query, ApiResponse>
        {
            private readonly IApplicationDbContext _context;
            private readonly IUserServiceHelper _userServices;
            public Handler(IApplicationDbContext context, IUserServiceHelper userServices)
            {
                _context = context;
                _userServices = userServices;
            }

            public async Task<ApiResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);
                    if (product == null)
                        return new ApiResponse("ProductId is not found", (int)HttpStatusCode.NotFound);

                    return new ApiResponse("Fetch data succeeded", product);
                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }

    }
}
