using CQRS.Web.Api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CQRS.Web.Api.Domain.Entities
{
    public class Requestion : BaseEntity
    {
        public Guid Id { get; set; }
        [Required]
        public DateTime DocumentDate { get; set; }
        [Required]
        public Status Status { get; set; }
        public virtual ICollection<RequestionDetail> Details { get; set; }
    }
}
