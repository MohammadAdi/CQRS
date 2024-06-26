using System.ComponentModel.DataAnnotations;

namespace CQRS.Web.Api.Domain.Entities
{
    public class Product : BaseEntity
    {
        public long ProductId { get; set; }
        [Required]
        public string ProductCode { get; set; }

        [StringLength(50)]
        [Required]
        public string ProductName { get; set; }
        [Required]
        public int Qty { get; set; }
        [Required]
        public decimal ProductPrice { get; set; }
    }
}
