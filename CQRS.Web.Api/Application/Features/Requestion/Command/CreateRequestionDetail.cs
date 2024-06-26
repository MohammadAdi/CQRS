using AutoWrapper.Wrappers;
using CQRS.Web.Api.Application.Common.Interface;
using CQRS.Web.Api.Application.Features.Requestion.Model;
using CQRS.Web.Api.Domain.Entities;
using CQRS.Web.Api.Domain.Enums;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Web.Api.Application.Features.Requestion.Command
{
    public class CreateRequestionDetail
    {
        public class Command : IRequest<ApiResponse>
        {
            public List<ItemRequestion> Details { get; set; }
            public string RequestionId { get; set; }
            public Status status { get; set; }
            public long UserId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
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

                    if (!request.Details.Any())
                        throw new ApiException("Requestion Detail cannot be empty");

                    var existingRequestion = await _context.Requestions.FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.RequestionId), cancellationToken);
                    if (existingRequestion == null)
                        return new ApiResponse("Requestion Not Found", statusCode: 404);

                    var existDetails = await _context.RequestionDetails.Where(x => x.RequestionId == Guid.Parse(request.RequestionId)).ToListAsync(cancellationToken);
                    if (existDetails.Any())
                        _context.RequestionDetails.RemoveRange(existDetails);

                    var groupDetail = request.Details.GroupBy(x => x.ProductId).Select(x => new { ProductId = x.Key, QtyOrder = x.Sum(y => y.QtyOrder) }).ToList();

                    var lists = new List<RequestionDetail>();
                    foreach (var item in groupDetail)
                    {
                        var d = new RequestionDetail()
                        {
                            Id = Guid.NewGuid(),
                            RequestionId = Guid.Parse(request.RequestionId),
                            ProductId = item.ProductId,
                            QtyOrder = item.QtyOrder,
                            IsDeleted = false,
                            CreatedBy = currentUser.Id
                        };
                        lists.Add(d);
                    }
                    _context.RequestionDetails.AddRange(lists);
                    await _context.SaveChangesAsync(cancellationToken);
                    return new ApiResponse("Requestion Detail has been created");
                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }

        }
    }
}