using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string MobileNumber { get; set; } = string.Empty;
    }
}
