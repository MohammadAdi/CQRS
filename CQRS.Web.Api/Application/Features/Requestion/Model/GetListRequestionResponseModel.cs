namespace CQRS.Web.Api.Application.Features.Requestion.Model
{
    public class GetListRequestionResponseModel
    {
        public int TotalData { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public List<GetRequestionModel> Data { get; set; }
    }

    public class GetRequestionModel
    {
        public string Id { get; set; }
        public string DocumentDate { get; set; }
        public string LastStatus { get; set; }
        public string LastModidiedBy { get; set; }
        public string LastModifiedDate { get; set; }
    }

}
