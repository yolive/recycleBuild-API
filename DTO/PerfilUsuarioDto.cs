namespace RecycleBuild.API.DTO
{
    public class PerfilUsuarioDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public double TotalMensal { get; set; }
        public int PontosMes { get; set; }
    }
}
