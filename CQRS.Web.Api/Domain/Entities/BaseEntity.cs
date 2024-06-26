using System.ComponentModel.DataAnnotations.Schema;

namespace CQRS.Web.Api.Domain.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User Creator { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual User Updater { get; set; }

    }
}
