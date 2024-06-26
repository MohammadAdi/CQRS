using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CQRS.Web.Api.Domain.Entities
{
    public class RequestionDetail : BaseEntity
    {
        public Guid Id { get; set; }
        [ForeignKey("RequestionId")]
        [Required]
        public Requestion Requestion { get; set; }
        public Guid RequestionId { get; set; }
        [ForeignKey("ProductId")]
        [Required]
        public Product Product { get; set; }
        public long ProductId { get; set; }
        [Required]
        public int QtyOrder { get; set; }

    }
}
