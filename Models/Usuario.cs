using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Farmacia.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string UsuarioNombre { get; set; }
        public string Password { get; set; }
        public string Estado { get; set; }
        public ICollection<Detalle_Permiso> Detalle_Permisos { get; set; }
        public ICollection<Detalle_Temp> Detalle_Temps { get; set; }
        public ICollection<Venta> Ventas { get; set; }

        [ForeignKey("Rol")]
        public int idrol { get; set; }
        [JsonIgnore]
        public Rol Rol { get; set; }

    }
}
