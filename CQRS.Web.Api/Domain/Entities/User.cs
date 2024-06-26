using CQRS.Web.Api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CQRS.Web.Api.Domain.Entities
{
    public class User
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime BirthOfDate { get; set; }
        [Required]
        public bool isActive { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
