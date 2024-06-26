namespace CQRS.Web.Api.Application.Features.Requestion.Model
{
    public class CreateRequestionDetailModel
    {
        public CreateRequestionDetailModel()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public Guid RequestionId { get; set; }
        public long ProductId { get; set; }
        public int QtyOrder { get; set; }

    }
}
