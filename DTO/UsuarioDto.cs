using System.ComponentModel.DataAnnotations;

namespace RecycleBuild.API.DTO
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        public string Nome { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Senha { get; set; }
    }
}
