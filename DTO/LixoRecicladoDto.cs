using System.ComponentModel.DataAnnotations;

namespace RecycleBuild.API.DTO
{
    public class LixoRecicladoDto
    {
        [Required]
        [Range(0.1, 1000)]
        public double Peso {  get; set; }
    }
}
