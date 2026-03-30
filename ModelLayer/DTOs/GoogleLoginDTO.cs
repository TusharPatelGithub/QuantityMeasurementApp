using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTOs
{
    public class GoogleLoginDTO
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
