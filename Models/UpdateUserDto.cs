using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Role is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Role must be between 3 and 50 characters")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password hash is required")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Password hash must be at least 8 characters")]
        public string PasswordHash { get; set; }

        [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters")]
        public string Department { get; set; }
    }
}