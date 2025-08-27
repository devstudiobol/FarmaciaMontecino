namespace Farmacia.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Telefono { get; set; }
        public string Direccion {  get; set; }
        public string Estado { get; set; }
        public ICollection<Venta> Ventas { get; set; }
    }
}
