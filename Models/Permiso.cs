namespace Farmacia.Models
{
    public class Permiso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public ICollection<Detalle_Permiso> Detalle_Permiso { get; set; }
    }
}
