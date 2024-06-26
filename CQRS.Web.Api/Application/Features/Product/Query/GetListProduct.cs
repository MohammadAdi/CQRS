using AutoWrapper.Wrappers;
using CQRS.Web.Api.Application.Common.Interface;
using CQRS.Web.Api.Application.Features.Product.Model;
using CQRS.Web.Api.Domain.Entities;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Web.Api.Application.Features.Product.Query
{
    public class GetListProduct
    {
        public class Query : IRequest<ApiResponse>
        {
            public string KeyWord { get; set; }
            public int? PageSize { get; set; }
            public int? PageNumber { get; set; }
            public long UserId { get; set; }
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
                    var currentUser = await _userServices.CheckCurrentUser(request.UserId, cancellationToken);

                    var listProduct = await _context.Products.AsNoTracking().Where(x => x.ProductCode.Contains(request.KeyWord) || x.ProductName.Contains(request.KeyWord)).ToListAsync(cancellationToken);
                     
                    if (!request.PageSize.HasValue && !request.PageNumber.HasValue)
                        return new ApiResponse("Fetch data succeeded", listProduct);

                    request.PageNumber = (request.PageNumber <= 0) ? 1 : request.PageNumber.Value;
                    request.PageSize = (request.PageSize <= 0) ? 10 : request.PageSize.Value;

                    var totalRecords = listProduct.Count;
                    var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize.Value);

                    var skip = (request.PageNumber - 1) * request.PageSize;

                    listProduct = listProduct.Skip(request.PageNumber.Value * request.PageSize.Value).Take(request.PageSize.Value).ToList();
                    var result = new GetListProductResponseModel()
                    {
                        TotalData = totalRecords,
                        TotalPage = totalPages,
                        PageNumber = request.PageNumber.Value,
                        PageSize = request.PageSize.Value,
                        Data = listProduct
                    };
                    return new ApiResponse("Fetch data succeeded", result);
                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }
    }
}