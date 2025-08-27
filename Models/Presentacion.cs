namespace Farmacia.Models
{
    public class Presentacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }
        public string Estado { get; set; }
        public ICollection<Producto> Productos { get; set; }
    }
}
