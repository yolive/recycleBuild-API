using System.ComponentModel.DataAnnotations;

namespace RecycleBuild.API.DTO
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Senha { get; set; }
    }
}
