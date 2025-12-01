using System.ComponentModel.DataAnnotations;

namespace ProyectoWebCommercialLopez.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "CI is required")]
        [StringLength(10, MinimumLength = 8,
            ErrorMessage = "CI must be between 8 and  10 characters")]
        public string? CI { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100   , MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Last name must be between 2 and 100 characters")]
        public string? LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateBirth { get; set; }

        [StringLength(200, MinimumLength = 5,
            ErrorMessage = "Address must be between 5 and 200 characters")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "State is required")]
        public byte State { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public User? User { get; set; }
    }
}