namespace Farmacia.Models
{
    public class Tipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public ICollection<Producto> Productos { get; set; }
    }
}
