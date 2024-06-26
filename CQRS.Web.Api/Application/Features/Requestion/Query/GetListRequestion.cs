using AutoWrapper.Wrappers;
using CQRS.Web.Api.Application.Common.Interface;
using CQRS.Web.Api.Application.Features.Product.Model;
using CQRS.Web.Api.Application.Features.Requestion.Model;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Web.Api.Application.Features.Requestion.Query
{
    public class GetListRequestion
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

                    var listProduct = await _context.Requestions.ToListAsync(cancellationToken);


                    if (!request.PageSize.HasValue && !request.PageNumber.HasValue)
                        return new ApiResponse("Fetch data succeeded", listProduct);

                    request.PageNumber = (request.PageNumber <= 0) ? 1 : request.PageNumber.Value;
                    request.PageSize = (request.PageSize <= 0) ? 10 : request.PageSize.Value;

                    var totalRecords = listProduct.Count;
                    var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize.Value);

                    var skip = (request.PageNumber.Value - 1) * request.PageSize.Value;

                    listProduct = listProduct.Skip(skip).Take(request.PageSize.Value).ToList();

                    var data = listProduct.Select(x => new GetRequestionModel()
                    {
                        Id = x.Id.ToString(),
                        DocumentDate = x.DocumentDate.ToString("yyyy-MM-dd"),
                        LastStatus = x.Status.ToString(),
                        LastModifiedDate = x.UpdatedAt.HasValue ? x.UpdatedAt.Value.ToString("yyyy-MM-dd") : x.CreatedAt.ToString("yyyy-MM-dd"),
                        LastModidiedBy = x.UpdatedAt.HasValue ? x.Updater.Name : x.Creator.Name,
                    }).ToList();

                    var result = new GetListRequestionResponseModel()
                    {
                        TotalData = totalRecords,
                        TotalPage = totalPages,
                        PageNumber = request.PageNumber.Value,
                        PageSize = request.PageSize.Value,
                        Data = data
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
