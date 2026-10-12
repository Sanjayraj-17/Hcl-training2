using System.ComponentModel.DataAnnotations;

namespace Employee_Management.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string JobTitle { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "999999999999.99")]
        public decimal Salary { get; set; }
    }
}