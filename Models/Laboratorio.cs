namespace Farmacia.Models
{
    public class Laboratorio
    {
        public int Id { get; set; }
        public string LaboratorioNombre { get; set; }
        public string Direccion { get; set; }
        public string Estado { get; set; }
        public ICollection<Producto> Productos { get; set; }

    }
}
