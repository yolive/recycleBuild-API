namespace RecycleBuild.API.Models
{
    public class LixoReciclado
    {
        public int id {  get; set; }
        public int IdUsuario { get; set; }
        public double Peso {  get; set; }
        public DateTime DataReciclagem { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
