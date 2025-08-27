using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Farmacia.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int Total { get; set; }
        public string Fecha { get; set; }
        public string Estado { get; set; }

        [ForeignKey("Usuario")]
        public int idusuario { get; set; }
        [JsonIgnore]
        public Usuario Usuario { get; set; }

        [ForeignKey("Cliente")]
        public int idcliente { get; set; }
        [JsonIgnore]
        public Cliente Cliente { get; set; }

        public ICollection<Detalle_Venta> Detalle_Venta { get; set; }
    }
}
