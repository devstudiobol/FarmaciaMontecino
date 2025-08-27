using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Farmacia.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Precio { get; set; }
        public int Stock { get; set; }
        public string Vencimiento { get; set; }
        public string Estado { get; set; }
        public ICollection<Detalle_Venta> Detalle_Ventas { get; set; }
        public ICollection<Detalle_Temp> Detalle_Temps { get; set; }
        public int concentracion { get; set; }
         
        public string casilla { get; set; }

        [ForeignKey("Presentacion")]
        public int idpresentacion { get; set; }
        [JsonIgnore]
        public Presentacion Presentacion { get; set; }

        [ForeignKey("Laboratorio")]
        public int idlaboratorio { get; set; }
        [JsonIgnore]
        public Laboratorio Laboratorio { get; set; }

        [ForeignKey("Tipo")]
        public int idtipo { get;set; }
        [JsonIgnore]
        public Tipo Tipo { get; set; }

    }
}
