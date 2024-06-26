namespace CQRS.Web.Api.Application.Features.Product.Model
{
    public class GetListProductResponseModel
    {
        public int TotalData { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public List<Domain.Entities.Product> Data { get; set; }
    }
}
