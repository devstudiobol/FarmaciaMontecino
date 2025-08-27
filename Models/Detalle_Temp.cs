using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Farmacia.Models
{
    public class Detalle_Temp
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public int Descuento { get; set; }
        public int Precio { get; set; }
        public int Total { get; set; }
        public string Estado { get; set; }

        [ForeignKey("Usuario")]
        public int idusuario { get; set; }
        [JsonIgnore]
        public Usuario Usuario { get; set; }

        [ForeignKey("Producto")]
        public int idproducto { get; set; }
        [JsonIgnore]
        public Producto Producto { get; set; }
    }
}
