using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoWebCommercialLopez.Models
{
    public class User
    {
        [Key]
        [ForeignKey("Person")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPerson { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 15,
            ErrorMessage = "Username must be between 3 and 50 characters")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(200, MinimumLength = 8,
            ErrorMessage = "Password must be between 6 and 150 characters")]
        public string? Password { get; set; }

        [Required]
        public int Role { get; set; }

        [Required]
        public byte StatePassword { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public Person? Person { get; set; }
    }
}
