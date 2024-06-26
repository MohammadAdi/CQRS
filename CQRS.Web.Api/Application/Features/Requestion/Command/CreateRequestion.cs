using AutoWrapper.Wrappers;
using CQRS.Web.Api.Application.Common.Interface;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using System.Transactions;

namespace CQRS.Web.Api.Application.Features.Requestion.Command
{
    public class CreateRequestion
    {
        public class Command : IRequest<ApiResponse>
        {
            public DateTime DocumentDate { get; set; }
            public long UserId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.DocumentDate).NotEmpty().WithMessage("Document Date cannot be null");
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

                    var newID = Guid.NewGuid();
                    var header = new Domain.Entities.Requestion
                    {
                        DocumentDate = request.DocumentDate.Date,
                        CreatedBy = currentUser.Id,
                        IsDeleted = false,
                        Status = Domain.Enums.Status.Draft
                    };
                    _context.Requestions.Add(header);

                    //if (!request.Details.Any())
                    //    throw new ApiException("Requestion Detail cannot be empty");

                    //var lists = new List<RequestionDetail>();
                    //foreach (var item in request.Details)
                    //{
                    //    var d = new RequestionDetail()
                    //    {
                    //        Id = Guid.NewGuid(),
                    //        RequestionId = newID,
                    //        ProductId = item.ProductId,
                    //        QtyOrder = item.QtyOrder,
                    //        IsDeleted = false,
                    //        CreatedBy = currentUser.Id
                    //    };
                    //    lists.Add(d);
                    //}
                    //_context.RequestionDetails.AddRange(lists);
                    await _context.SaveChangesAsync(cancellationToken);

                    return new ApiResponse("Requestion has been created");
                }
                catch (Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }
    }
}
