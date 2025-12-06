using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.DataAccess.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        public byte[]? LocationImage { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Building> Buildings { get; set; } = new List<Building>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Resource> Resources { get; set; } = new List<Resource>();
    }
}









